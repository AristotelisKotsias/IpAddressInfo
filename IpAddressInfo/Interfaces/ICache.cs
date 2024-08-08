namespace IpAddressInfo.Interfaces;

public interface ICache
{
    bool TryGetValue<TItem>(object key, out TItem value);
    void Set<TItem>(object key, TItem value, TimeSpan absoluteExpirationRelativeToNow);
}