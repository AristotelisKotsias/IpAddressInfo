using IpAddressInfo.Interfaces;

namespace IpAddressInfo.Services;

public class ExternalIPService : IExternalIPService
{
    private readonly HttpClient _httpClient;

    public ExternalIPService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> FetchIPAddressDetailsAsync(string ip)
    {
        var response = await _httpClient.GetAsync($"https://ip2c.org/{ip}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var parts = content.Split(';');
            if (parts is ["1", _, _, _])
            {
                return content; 
            }
        }
        return null;
    }
} 