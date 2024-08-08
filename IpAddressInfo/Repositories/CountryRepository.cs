#region

using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using Microsoft.EntityFrameworkCore;

#endregion

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
        ArgumentNullException.ThrowIfNull(country);
        var existingCountry = await _context.Countries
            .AnyAsync(c => c.Name == country.Name);
        if (existingCountry)
        {
            throw new DbUpdateException($"A country with the name '{country.Name}' already exists.", new Exception("Unique constraint violation"));
        }
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();
    }
}