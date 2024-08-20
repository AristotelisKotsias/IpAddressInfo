#region

using IpAddressInfo.Interfaces;

#endregion

namespace IpAddressInfo.Services;

public class ExternalIpService : IExternalIpService
{
    private readonly string? _baseUrl;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ExternalIpService> _logger;

    public ExternalIpService(ILogger<ExternalIpService> logger, IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration["IpService:BaseUrl"];
    }

    public async Task<string?> FetchIpAddressDetailsAsync(string ip)
    {
        var client = _httpClientFactory.CreateClient("Ip2cService");
        try
        {
            var response = await client.GetAsync(ip);
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