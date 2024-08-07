using IpAddressInfo.Dtos;
using IpAddressInfo.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpAddressInfo.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<CountryReportDto>>> GetCountryReport(
        [FromBody] List<string>? countryCodes = null)
    {
        if (countryCodes == null || countryCodes.Count == 0) countryCodes = null;
        var result = await _reportService.GetCountryReportAsync(countryCodes);
        return Ok(result);
    }
}