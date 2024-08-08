namespace IpAddressInfo.Interfaces;

public interface IExternalIpService
{
    Task<string?> FetchIpAddressDetailsAsync(string ip);
}