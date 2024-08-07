using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpAddressInfo.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly AppDbContext _context;

    public CountryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Country?> GetCountryByNameAsync(string name)
    {
        return await _context.Countries
            .FirstOrDefaultAsync(country => country.Name == name);
    }
    
    public async Task AddCountryAsync(Country country)
    {
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();
    }
}