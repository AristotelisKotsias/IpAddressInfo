#region

using IpAddressInfo.Dtos;
using IpAddressInfo.Interfaces;
using Microsoft.AspNetCore.Mvc;

#endregion

[ApiController]
[Route("[controller]")]
public class RedisController : ControllerBase
{
    private readonly IRedisCacheService _cache;
    private readonly ILogger<RedisController> _logger;

    public RedisController(ILogger<RedisController> logger, IRedisCacheService cache)
    {
        _logger = logger;
        _cache = cache;
    }

    [HttpGet(Name = "GetCache")]
    public async Task<IActionResult> GetCache(string key)
    {
        var res = await _cache.GetAsync(key);
        return Ok(res);
    }

    [HttpPost(Name = "AddToCache")]
    public async Task<IActionResult> AddToCache(string key, IpAddressDto val)
    {
        await _cache.SetAsync(key, val);
        return Ok(key);
    }
}