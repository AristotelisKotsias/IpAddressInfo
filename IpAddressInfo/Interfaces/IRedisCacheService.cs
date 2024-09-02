#region

using IpAddressInfo.Dtos;

#endregion

namespace IpAddressInfo.Interfaces;

public interface IRedisCacheService
{
    Task SetAsync(string key, IpAddressDto value, TimeSpan? expiry = null);
    Task<IpAddressDto?> GetAsync(string key);
    Task DeleteAsync(string key);
}