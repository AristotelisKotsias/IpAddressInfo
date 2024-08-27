#region

using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using Microsoft.EntityFrameworkCore;

#endregion

namespace IpAddressInfo.Repositories;

public class CountryRepository(IDbContextFactory<AppDbContext> context) : ICountryRepository
{
    public async Task<Country?> GetCountryByNameAsync(string name)
    {
        await using var ctx = await context.CreateDbContextAsync();
        return await ctx.Countries.AsNoTracking().FirstOrDefaultAsync(x => x.Name.Equals(name));
    }

    public async Task AddCountryAsync(Country country)
    {
        await using var ctx = await context.CreateDbContextAsync();
        var x = await ctx.Countries.FirstOrDefaultAsync(x => x.Name.Equals(country.Name));
        if (x is not null)
        {
            x.ThreeLetterCode = country.ThreeLetterCode;
            x.CreatedAt = country.CreatedAt;
            x.TwoLetterCode = country.TwoLetterCode;
            ctx.Countries.Update(x);
        }

        x = country;

        await ctx.Countries.AddAsync(x);
        await ctx.SaveChangesAsync();
    }
}