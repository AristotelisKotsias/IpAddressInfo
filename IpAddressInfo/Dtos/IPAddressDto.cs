namespace IpAddressInfo.Dtos;

public class IPAddressDto
{
    public string IP { get; set; } = default!;
    public string CountryName { get; set; } = default!;
    public string TwoLetterCode { get; set; } = default!;
    public string ThreeLetterCode { get; set; } = default!;
}