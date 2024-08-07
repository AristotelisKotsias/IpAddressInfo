namespace IpAddressInfo.Interfaces;

public interface IExternalIPService
{
    Task<string?> FetchIPAddressDetailsAsync(string ip);
}