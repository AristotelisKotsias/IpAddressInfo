#region

using IpAddressInfo.Interfaces;

#endregion

namespace IpAddressInfo.Services;

public class ExternalIpService : IExternalIpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalIpService> _logger;

    public ExternalIpService(HttpClient httpClient, ILogger<ExternalIpService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string?> FetchIpAddressDetailsAsync(string ip)
    {
        try
        {
            var response = await _httpClient.GetAsync($"https://ip2c.org/{ip}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var parts = content.Split(';');
                if (parts is ["1", _, _, _]) return content;
            }
            else
            {
                _logger.LogWarning("Failed to fetch IP address details for {IP}. Status Code: {StatusCode}", ip,
                    response.StatusCode);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request error while fetching IP address details for {IP}", ip);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while fetching IP address details for {IP}", ip);
        }

        return null;
    }
}