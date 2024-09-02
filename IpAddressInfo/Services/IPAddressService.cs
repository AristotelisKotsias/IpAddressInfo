#region

#region

using System.Runtime.CompilerServices;
using IpAddressInfo.Dtos;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;

#endregion

[assembly: InternalsVisibleTo("IpAddressInfo.Tests")]

#endregion

namespace IpAddressInfo.Services;

public class IpAddressService : IIpAddressService
{
    private readonly ICountryRepository _countryRepository;
    private readonly IExternalIpService _externalIpService;
    private readonly IIpRepository _ipRepository;
    private readonly ILogger<IpAddressService> _logger;
    private readonly IRedisCacheService _redisCache;

    public IpAddressService(
        IIpRepository ipRepository,
        IExternalIpService externalIpService,
        ICountryRepository countryRepository,
        ILogger<IpAddressService> logger, IRedisCacheService redisCache)
    {
        _ipRepository = ipRepository;
        _externalIpService = externalIpService;
        _countryRepository = countryRepository;
        _logger = logger;
        _redisCache = redisCache;
    }

    public async Task<IpAddressDto?> GetIpAddressDetailsAsync(string ip)
    {
        try
        {
            var cachedIpInfo = await _redisCache.GetAsync(ip);
            if (cachedIpInfo is not null) return cachedIpInfo;

            var ipAddressDto = await GetIpAddressFromDatabaseAsync(ip);
            if (ipAddressDto is not null) return ipAddressDto;

            ipAddressDto = await GetIpAddressFromExternalServiceAsync(ip);
            if (ipAddressDto is not null) return ipAddressDto;
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
        if (ipAddress is null) return null;
        var ipAddressDto = new IpAddressDto
        {
            IP = ipAddress.IP,
            CountryName = ipAddress.Country.Name,
            TwoLetterCode = ipAddress.Country.TwoLetterCode,
            ThreeLetterCode = ipAddress.Country.ThreeLetterCode
        };

        await _redisCache.SetAsync(ip, ipAddressDto, TimeSpan.FromHours(1));
        return ipAddressDto;
    }

    private async Task<IpAddressDto?> GetIpAddressFromExternalServiceAsync(string ip)
    {
        var rawResponse = await _externalIpService.FetchIpAddressDetailsAsync(ip);
        if(rawResponse is null) return null;
        var parts = rawResponse.Split(';');
        var ipAddressDto = new IpAddressDto
        {
            IP = ip,
            CountryName = parts[3],
            TwoLetterCode = parts[1],
            ThreeLetterCode = parts[2]
        };

        var country = await _countryRepository.GetCountryByNameAsync(ipAddressDto.CountryName);
        if (country == null)
        {
            country = new Country
            {
                Name = ipAddressDto.CountryName,
                TwoLetterCode = ipAddressDto.TwoLetterCode,
                ThreeLetterCode = ipAddressDto.ThreeLetterCode,
                CreatedAt = DateTime.UtcNow
            };
            await _countryRepository.AddCountryAsync(country);
        }

        var newIpAddress = new IPAddress
        {
            IP = ipAddressDto.IP,
            CountryId = country.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _ipRepository.AddIpAddressAsync(newIpAddress);
        await _redisCache.SetAsync(ip, ipAddressDto, TimeSpan.FromHours(1));

        return null;
    }
}