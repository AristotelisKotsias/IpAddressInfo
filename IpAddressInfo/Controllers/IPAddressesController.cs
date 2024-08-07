using IpAddressInfo.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpAddressInfo.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class IPAddressesController : ControllerBase
{
    private readonly IIPAddressService _ipAddressService;

    public IPAddressesController(IIPAddressService ipAddressService)
    {
        _ipAddressService = ipAddressService;
    }


    [HttpGet("{ip}")]
    public async Task<IActionResult> GetIpInfo(string ip)
    {
        var ipInfo = await _ipAddressService.GetIPAddressDetailsAsync(ip);
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