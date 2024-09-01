using System.Text;
using System.Text.Json;
using IpAddressInfo.Dtos;
using IpAddressInfo.Interfaces;
using StackExchange.Redis;

namespace IpAddressInfo.Data;

public class RedisCacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheService> logger) : IRedisCacheService
{
    
    public async Task SetAsync(string key, IpAddressDto? ipAddressDto)
    {
        var jsonData = JsonSerializer.Serialize(ipAddressDto);
        var db = connectionMultiplexer.GetDatabase();
        await db.StringSetAsync(key, jsonData, TimeSpan.FromMinutes(5));
        logger.LogInformation($"Cache set for key: {key}");
    }
    
    public async Task<IpAddressDto?> GetAsync(string key)
    {
        var db = connectionMultiplexer.GetDatabase();
        var jsonData = await db.StringGetAsync(key);
        if (jsonData.IsNullOrEmpty)
        {
            logger.LogInformation($"Cache miss for key: {key}");
            return null;
        }

        logger.LogInformation($"Cache hit for key: {key}");
        return JsonSerializer.Deserialize<IpAddressDto>(jsonData);
    }

}