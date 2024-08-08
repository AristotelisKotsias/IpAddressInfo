#region

using IpAddressInfo.Dtos;

#endregion

namespace IpAddressInfo.Interfaces;

public interface IIpAddressService
{
    Task<IpAddressDto?> GetIpAddressDetailsAsync(string ip);
}