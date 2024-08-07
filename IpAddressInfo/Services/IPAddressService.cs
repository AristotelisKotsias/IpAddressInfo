using IpAddressInfo.Dtos;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace IpAddressInfo.Services;

public class IPAddressService : IIPAddressService
{
    private readonly IIPRepository _ipRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IMemoryCache _cache;
    private readonly IExternalIPService _externalIPService;

    public IPAddressService(IIPRepository ipRepository, IMemoryCache cache, IExternalIPService externalIPService, ICountryRepository countryRepository)
    {
        _ipRepository = ipRepository;
        _cache = cache;
        _externalIPService = externalIPService;
        _countryRepository = countryRepository;
    }

    public async Task<IPAddressDto?> GetIPAddressDetailsAsync(string ip)
    {
        if (_cache.TryGetValue(ip, out IPAddressDto? cachedIPInfo))
        {
            return cachedIPInfo;
        }
        
        // Check database
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

            // Store in cache
            _cache.Set(ip, ipAddressDto, TimeSpan.FromHours(1));

            return ipAddressDto;
        }
        // Fetch from external service
        var rawResponse = await _externalIPService.FetchIPAddressDetailsAsync(ip);
        if (rawResponse != null)
        {
            var parts = rawResponse.Split(';');
            if (parts.Length == 4 && parts[0] == "1")
            {
                var ipInfoFromService = new IPAddressDto
                {
                    IP = ip,
                    CountryName = parts[3],
                    TwoLetterCode = parts[1],
                    ThreeLetterCode = parts[2]
                };

                // Persist to database
                var country = await _countryRepository.GetCountryByNameAsync(ipInfoFromService.CountryName);
                if (country == null)
                {
                    country = new Country
                    {
                        Name = ipInfoFromService.CountryName,
                        TwoLetterCode = ipInfoFromService.TwoLetterCode,
                        ThreeLetterCode = ipInfoFromService.ThreeLetterCode,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _countryRepository.AddCountryAsync(country);
                }

                var newIpAddress = new IPAddress
                {
                    IP = ipInfoFromService.IP,
                    CountryId = country.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _ipRepository.AddIPAddressAsync(newIpAddress);

                // Store in cache
                _cache.Set(ip, ipInfoFromService, TimeSpan.FromHours(1));
                return ipInfoFromService;
            }
        }


        return null;
    }
    
    
}