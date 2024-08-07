using IpAddressInfo.Entities;

namespace IpAddressInfo.Interfaces;

public interface ICountryRepository
{
    Task<Country?> GetCountryByNameAsync(string name);
    Task AddCountryAsync(Country country);
}