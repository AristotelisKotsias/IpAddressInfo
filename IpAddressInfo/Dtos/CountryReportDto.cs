namespace IpAddressInfo.Dtos;

public class CountryReportDto
{
    public string CountryName { get; set; }
    public int AddressesCount { get; set; }
    public DateTime LastAddressUpdated { get; set; }
}