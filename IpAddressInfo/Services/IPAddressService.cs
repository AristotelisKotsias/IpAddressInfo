using IpAddressInfo.Dtos;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace IpAddressInfo.Services;

public class IPAddressService : IIPAddressService
{
    private readonly IMemoryCache _cache;
    private readonly ICountryRepository _countryRepository;
    private readonly IExternalIPService _externalIPService;
    private readonly IIPRepository _ipRepository;
    private readonly ILogger<IPAddressService> _logger;

    public IPAddressService(
        IIPRepository ipRepository,
        IMemoryCache cache,
        IExternalIPService externalIPService,
        ICountryRepository countryRepository,
        ILogger<IPAddressService> logger)
    {
        _ipRepository = ipRepository;
        _cache = cache;
        _externalIPService = externalIPService;
        _countryRepository = countryRepository;
        _logger = logger;
    }

    public async Task<IPAddressDto?> GetIPAddressDetailsAsync(string ip)
    {
        try
        {
            if (_cache.TryGetValue(ip, out IPAddressDto? cachedIPInfo))
            {
                return cachedIPInfo;
            }

            var ipAddressDto = await GetIPAddressFromDatabaseAsync(ip);
            if (ipAddressDto != null)
            {
                return ipAddressDto;
            }

            ipAddressDto = await GetIPAddressFromExternalServiceAsync(ip);
            if (ipAddressDto != null)
            {
                await SaveIPAddressToDatabaseAsync(ipAddressDto);
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

    private async Task<IPAddressDto?> GetIPAddressFromDatabaseAsync(string ip)
    {
        var ipAddress = await _ipRepository.GetIPAddressByIPAsync(ip);
        if (ipAddress != null)
        {
            var ipAddressDto = new IPAddressDto
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

    private async Task<IPAddressDto?> GetIPAddressFromExternalServiceAsync(string ip)
    {
        var rawResponse = await _externalIPService.FetchIPAddressDetailsAsync(ip);
        if (rawResponse != null)
        {
            var parts = rawResponse.Split(';');
            if (parts is ["1", _, _, _])
                return new IPAddressDto
                {
                    IP = ip,
                    CountryName = parts[3],
                    TwoLetterCode = parts[1],
                    ThreeLetterCode = parts[2]
                };
        }

        return null;
    }

    private async Task SaveIPAddressToDatabaseAsync(IPAddressDto ipInfo)
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
        await _ipRepository.AddIPAddressAsync(newIpAddress);
    }
}