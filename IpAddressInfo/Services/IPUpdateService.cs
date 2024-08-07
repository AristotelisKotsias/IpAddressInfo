using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace IpAddressInfo.Services;

public class IPUpdateService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);
    private readonly ILogger<IPUpdateService> _logger;


    public IPUpdateService(ILogger<IPUpdateService> logger, IServiceProvider serviceProvider, IMemoryCache cache)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("IP Update Service job started at {Time}", DateTimeOffset.Now);
            await UpdateIPAddressesAsync();
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task UpdateIPAddressesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var ipRepository = scope.ServiceProvider.GetRequiredService<IIPRepository>();
        var externalIPService = scope.ServiceProvider.GetRequiredService<IExternalIPService>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        const int batchSize = 100;
        var skip = 0;
        while (true)
        {
            var ipAddresses = await ipRepository.GetIPAddressesInBatchAsync(skip, batchSize);
            if (ipAddresses.Count == 0)
                break;

            foreach (var ipAddress in ipAddresses)
            {
                var rawResponse = await externalIPService.FetchIPAddressDetailsAsync(ipAddress.IP);
                if (rawResponse == null) continue;
                var parts = rawResponse.Split(';');
                if (parts is not ["1", _, _, _]) continue;
                var newCountryName = parts[3];
                var newTwoLetterCode = parts[1];
                var newThreeLetterCode = parts[2];

                if (ipAddress.Country.Name == newCountryName &&
                    ipAddress.Country.TwoLetterCode == newTwoLetterCode &&
                    ipAddress.Country.ThreeLetterCode == newThreeLetterCode) continue;
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

            await context.SaveChangesAsync();
            skip += batchSize;
        }
        _logger.LogInformation("IP Update Service job completed at {Time}", DateTimeOffset.Now);
    }
}
