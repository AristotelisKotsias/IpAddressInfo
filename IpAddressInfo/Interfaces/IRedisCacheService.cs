using IpAddressInfo.Dtos;

namespace IpAddressInfo.Interfaces;

public interface IRedisCacheService
{
    Task SetAsync(string key, IpAddressDto value);
    Task<IpAddressDto?> GetAsync(string key);
}