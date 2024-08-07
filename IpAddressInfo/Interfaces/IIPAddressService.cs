using IpAddressInfo.Dtos;

namespace IpAddressInfo.Interfaces;

public interface IIPAddressService
{
    Task<IPAddressDto?> GetIPAddressDetailsAsync(string ip);
}