#region

using System.Text.Json;
using IpAddressInfo.Dtos;
using IpAddressInfo.Interfaces;
using StackExchange.Redis;

#endregion

namespace IpAddressInfo.Data;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _db;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheService> logger)
    {
        _logger = logger;
        _db = connectionMultiplexer.GetDatabase();
    }

    public async Task SetAsync(string key, IpAddressDto? ipAddressDto, TimeSpan? expiry = null)
    {
        var jsonData = JsonSerializer.Serialize(ipAddressDto);
        await _db.StringSetAsync(key, jsonData, expiry);
        _logger.LogInformation($"Cache set for key: {key}");
    }

    public async Task<IpAddressDto?> GetAsync(string key)
    {
        var jsonData = await _db.StringGetAsync(key);
        if (jsonData.IsNullOrEmpty)
        {
            _logger.LogInformation($"Cache miss for key: {key}");
            return null;
        }

        _logger.LogInformation($"Cache hit for key: {key}");
        return JsonSerializer.Deserialize<IpAddressDto>(jsonData!);
    }

    public async Task DeleteAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
        _logger.LogInformation($"Entry removed: {key}");
    }
}