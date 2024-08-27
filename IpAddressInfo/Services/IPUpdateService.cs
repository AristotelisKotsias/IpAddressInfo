#region

using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

#endregion

namespace IpAddressInfo.Services;

public class IpUpdateService : BackgroundService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);
    private readonly ILogger<IpUpdateService> _logger;
    private readonly IIpRepository _ipRepository;
    private readonly IExternalIpService _exIp;
    private readonly ICountryRepository _countryRepository;


    public IpUpdateService(ILogger<IpUpdateService> logger, IMemoryCache cache, IIpRepository ipRepository,
        IExternalIpService exIp, ICountryRepository countryRepository)
    {
        _logger = logger;
        _cache = cache;
        _ipRepository = ipRepository;
        _exIp = exIp;
        _countryRepository = countryRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            _logger.LogInformation("IP Update Service job started at {Time}", DateTimeOffset.Now);
            await UpdateIpAddressesAsync();
            await Task.Delay(_interval, ct);
        }
    }

    private async Task UpdateIpAddressesAsync()
    {
        const int batchSize = 100;
        var skip = 0;
        var countryCache = new Dictionary<string, Country>(); // Cache to store country data
        while (true)
        {
            try
            {
                var ipAddresses = await _ipRepository.GetIpAddressesInBatchAsync(skip, batchSize);
                if (ipAddresses.Count == 0)
                    break;
                var tasks = ipAddresses.Select(async ipAddress =>
                {
                    try
                    {
                        var rawResponse = await _exIp.FetchIpAddressDetailsAsync(ipAddress.IP);
                        if (rawResponse == null)
                        {
                            _logger.LogWarning("Failed to fetch details for IP: {IP}", ipAddress.IP);
                            return;
                        }

                        var parts = rawResponse.Split(';');
                        var newCountryName = parts[3];
                        var newTwoLetterCode = parts[1];
                        var newThreeLetterCode = parts[2];

                        if (ipAddress.Country.Name == newCountryName &&
                            ipAddress.Country.TwoLetterCode == newTwoLetterCode &&
                            ipAddress.Country.ThreeLetterCode == newThreeLetterCode)
                            return;

                        if (!countryCache.TryGetValue(newCountryName, out var country))
                        {
                            if (country == null)
                            {
                                country = new Country
                                {
                                    Name = newCountryName,
                                    TwoLetterCode = newTwoLetterCode,
                                    ThreeLetterCode = newThreeLetterCode,
                                    CreatedAt = DateTime.UtcNow
                                };
                            }

                            await _countryRepository.AddCountryAsync(country);
                            countryCache[newCountryName] = country; 
                        }

                        ipAddress.CountryId = country.Id;
                        ipAddress.UpdatedAt = DateTime.UtcNow;
                        _cache.Remove(ipAddress.IP);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing IP address: {IP}", ipAddress.IP);
                    }
                });

                await Task.WhenAll(tasks);
                await _ipRepository.UpdateIpAddresses(ipAddresses);
                skip += batchSize;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating IP addresses in batch starting at skip value: {Skip}", skip);
                break;
            }
        }

        _logger.LogInformation("IP Update Service job completed at {Time}", DateTimeOffset.Now);
    }
}