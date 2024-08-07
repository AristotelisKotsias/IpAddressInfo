using IpAddressInfo.Interfaces;

namespace IpAddressInfo.Services
{
    public class ExternalIPService : IExternalIPService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalIPService> _logger;

        public ExternalIPService(HttpClient httpClient, ILogger<ExternalIPService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string?> FetchIPAddressDetailsAsync(string ip)
        {
            try
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
                else
                {
                    _logger.LogWarning("Failed to fetch IP address details for {IP}. Status Code: {StatusCode}", ip, response.StatusCode);
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
}