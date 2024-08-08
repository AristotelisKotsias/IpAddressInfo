#region

using IpAddressInfo.Entities;

#endregion

namespace IpAddressInfo.Interfaces;

public interface IIpRepository
{
    Task<IPAddress?> GetIpAddressByIpAsync(string ip);
    Task AddIpAddressAsync(IPAddress ipAddress);
    Task<List<IPAddress>> GetIpAddressesInBatchAsync(int skip, int take);
}