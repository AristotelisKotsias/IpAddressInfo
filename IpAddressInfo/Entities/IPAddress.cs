namespace IpAddressInfo.Entities;

public class IPAddress
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    public string IP { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Country Country { get; set; }
}