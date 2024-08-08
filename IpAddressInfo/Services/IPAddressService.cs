#region

using System.Runtime.CompilerServices;
using IpAddressInfo.Dtos;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
[assembly: InternalsVisibleTo("IpAddressInfo.Tests")]

#endregion

namespace IpAddressInfo.Services;

public class IpAddressService : IIpAddressService
{
    private readonly ICache _cache;
    private readonly ICountryRepository _countryRepository;
    private readonly IExternalIpService _externalIpService;
    private readonly IIpRepository _ipRepository;
    private readonly ILogger<IpAddressService> _logger;

    public IpAddressService(
        IIpRepository ipRepository,
        ICache cache,
        IExternalIpService externalIpService,
        ICountryRepository countryRepository,
        ILogger<IpAddressService> logger)
    {
        _ipRepository = ipRepository;
        _cache = cache;
        _externalIpService = externalIpService;
        _countryRepository = countryRepository;
        _logger = logger;
    }

    public async Task<IpAddressDto?> GetIpAddressDetailsAsync(string ip)
    {
        try
        {
            if (_cache.TryGetValue(ip, out IpAddressDto? cachedIpInfo)) return cachedIpInfo;

            var ipAddressDto = await GetIpAddressFromDatabaseAsync(ip);
            if (ipAddressDto != null) return ipAddressDto;

            ipAddressDto = await GetIpAddressFromExternalServiceAsync(ip);
            if (ipAddressDto != null)
            {
                await SaveIpAddressToDatabaseAsync(ipAddressDto);
                _cache.Set(ip, ipAddressDto, TimeSpan.FromHours(1));
                return ipAddressDto;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting IP address details.");
        }

        return null;
    }

    private async Task<IpAddressDto?> GetIpAddressFromDatabaseAsync(string ip)
    {
        var ipAddress = await _ipRepository.GetIpAddressByIpAsync(ip);
        if (ipAddress != null)
        {
            var ipAddressDto = new IpAddressDto
            {
                IP = ipAddress.IP,
                CountryName = ipAddress.Country.Name,
                TwoLetterCode = ipAddress.Country.TwoLetterCode,
                ThreeLetterCode = ipAddress.Country.ThreeLetterCode
            };

            _cache.Set(ip, ipAddressDto, TimeSpan.FromHours(1));
            return ipAddressDto;
        }

        return null;
    }

    private async Task<IpAddressDto?> GetIpAddressFromExternalServiceAsync(string ip)
    {
        var rawResponse = await _externalIpService.FetchIpAddressDetailsAsync(ip);
        var parts = rawResponse?.Split(';');
        if (parts is ["1", _, _, _])
            return new IpAddressDto
            {
                IP = ip,
                CountryName = parts[3],
                TwoLetterCode = parts[1],
                ThreeLetterCode = parts[2]
            };

        return null;
    }

    internal async Task SaveIpAddressToDatabaseAsync(IpAddressDto ipInfo)
    {
        var country = await _countryRepository.GetCountryByNameAsync(ipInfo.CountryName);
        if (country == null)
        {
            country = new Country
            {
                Name = ipInfo.CountryName,
                TwoLetterCode = ipInfo.TwoLetterCode,
                ThreeLetterCode = ipInfo.ThreeLetterCode,
                CreatedAt = DateTime.UtcNow
            };
            await _countryRepository.AddCountryAsync(country);
        }

        var newIpAddress = new IPAddress
        {
            IP = ipInfo.IP,
            CountryId = country.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _ipRepository.AddIpAddressAsync(newIpAddress);
    }
}