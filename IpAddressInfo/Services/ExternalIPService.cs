#region

using IpAddressInfo.Interfaces;

#endregion

namespace IpAddressInfo.Services;

public class ExternalIpService(
    ILogger<ExternalIpService> logger,
    IHttpClientFactory httpClientFactory)
    : IExternalIpService
{
    public async Task<string?> FetchIpAddressDetailsAsync(string ip)
    {
        var client = httpClientFactory.CreateClient("Ip2cService");
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
                logger.LogWarning("Failed to fetch IP address details for {IP}. Status Code: {StatusCode}", ip,
                    response.StatusCode);
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request error while fetching IP address details for {IP}", ip);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while fetching IP address details for {IP}", ip);
        }

        return null;
    }
}