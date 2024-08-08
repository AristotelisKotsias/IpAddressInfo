#region

using IpAddressInfo.Dtos;

#endregion

namespace IpAddressInfo.Interfaces;

public interface IReportService
{
    Task<IEnumerable<CountryReportDto>> GetCountryReportAsync(IEnumerable<string>? countryCodes);
}