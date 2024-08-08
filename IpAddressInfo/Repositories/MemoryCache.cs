#region

using IpAddressInfo.Interfaces;
using Microsoft.Extensions.Caching.Memory;

#endregion

namespace IpAddressInfo.Repositories;

public class MemoryCache : ICache
{
    private readonly IMemoryCache _cache;

    public MemoryCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public bool TryGetValue<TItem>(object key, out TItem value)
    {
        return _cache.TryGetValue(key, out value);
    }

    public void Set<TItem>(object key, TItem value, TimeSpan absoluteExpirationRelativeToNow)
    {
        _cache.Set(key, value, absoluteExpirationRelativeToNow);
    }
}