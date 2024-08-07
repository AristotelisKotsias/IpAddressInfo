using IpAddressInfo.Entities;

namespace IpAddressInfo.Interfaces;

public interface IIPRepository
{
    Task<IPAddress?> GetIPAddressByIPAsync(string ip);
    Task AddIPAddressAsync(IPAddress ipAddress);
    Task<List<IPAddress>> GetIPAddressesInBatchAsync(int skip, int take);
}