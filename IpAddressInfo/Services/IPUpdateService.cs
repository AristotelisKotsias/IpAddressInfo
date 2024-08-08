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
    private readonly IServiceProvider _serviceProvider;

    public IpUpdateService(ILogger<IpUpdateService> logger, IServiceProvider serviceProvider, IMemoryCache cache)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _cache = cache;
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
        using var scope = _serviceProvider.CreateScope();
        var ipRepository = scope.ServiceProvider.GetRequiredService<IIpRepository>();
        var externalIpService = scope.ServiceProvider.GetRequiredService<IExternalIpService>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        const int batchSize = 100;
        var skip = 0;
        while (true)
            try
            {
                var ipAddresses = await ipRepository.GetIpAddressesInBatchAsync(skip, batchSize);
                if (ipAddresses.Count == 0)
                    break;

                foreach (var ipAddress in ipAddresses)
                    try
                    {
                        var rawResponse = await externalIpService.FetchIpAddressDetailsAsync(ipAddress.IP);
                        if (rawResponse == null)
                        {
                            _logger.LogWarning("Failed to fetch details for IP: {IP}", ipAddress.IP);
                            continue;
                        }

                        var parts = rawResponse.Split(';');
                        var newCountryName = parts[3];
                        var newTwoLetterCode = parts[1];
                        var newThreeLetterCode = parts[2];

                        if (ipAddress.Country.Name == newCountryName &&
                            ipAddress.Country.TwoLetterCode == newTwoLetterCode &&
                            ipAddress.Country.ThreeLetterCode == newThreeLetterCode)
                            continue;

                        var country = await context.Countries.FirstOrDefaultAsync(c => c.Name == newCountryName);
                        if (country == null)
                        {
                            country = new Country
                            {
                                Name = newCountryName,
                                TwoLetterCode = newTwoLetterCode,
                                ThreeLetterCode = newThreeLetterCode,
                                CreatedAt = DateTime.UtcNow
                            };
                            context.Countries.Add(country);
                            await context.SaveChangesAsync();
                        }

                        ipAddress.CountryId = country.Id;
                        ipAddress.UpdatedAt = DateTime.UtcNow;
                        context.IPAddresses.Update(ipAddress);

                        // Invalidate cache
                        _cache.Remove(ipAddress.IP);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing IP address: {IP}", ipAddress.IP);
                    }

                await context.SaveChangesAsync();
                skip += batchSize;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating IP addresses in batch starting at skip value: {Skip}", skip);
                break;
            }

        _logger.LogInformation("IP Update Service job completed at {Time}", DateTimeOffset.Now);
    }
}