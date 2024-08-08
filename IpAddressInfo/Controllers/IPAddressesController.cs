#region

using IpAddressInfo.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Net = System.Net;

#endregion

namespace IpAddressInfo.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class IpAddressesController : ControllerBase
{
    private readonly IIpAddressService _ipAddressService;

    public IpAddressesController(IIpAddressService ipAddressService)
    {
        _ipAddressService = ipAddressService;
    }


    [HttpGet("{ip}")]
    public async Task<IActionResult> GetIpInfo(string ip)
    {
        if (!Net.IPAddress.TryParse(ip, out var parsedIp)) return BadRequest("Malformed IP");
        var ipInfo = await _ipAddressService.GetIpAddressDetailsAsync(parsedIp.ToString());
        if (ipInfo != null) return Ok(ipInfo);
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "IP info not found",
            Detail = $"IP info with ip {ip} not found.",
            Instance = HttpContext.Request.Path
        };
        return NotFound(problemDetails);
    }
}