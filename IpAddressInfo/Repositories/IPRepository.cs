#region

using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using Microsoft.EntityFrameworkCore;

#endregion

namespace IpAddressInfo.Repositories;

public class IpRepository(IDbContextFactory<AppDbContext> context) : IIpRepository
{
    public async Task<IPAddress?> GetIpAddressByIpAsync(string ip)
    {
        await using var ctx = await context.CreateDbContextAsync();
        return await ctx.IPAddresses
            .Include(ipAddress => ipAddress.Country)
            .FirstOrDefaultAsync(ipAddress => ipAddress.IP == ip);
    }

    public async Task AddIpAddressAsync(IPAddress ipAddress)
    {
        await using var ctx = await context.CreateDbContextAsync();
        ctx.IPAddresses.Add(ipAddress);
        await ctx.SaveChangesAsync();
    }

    public async Task<List<IPAddress>> GetIpAddressesInBatchAsync(int skip, int take)
    {
        await using var ctx = await context.CreateDbContextAsync();
        return await ctx.IPAddresses
            .Include(ipAddress => ipAddress.Country)
            .OrderBy(ipAddress => ipAddress.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task UpdateIpAddresses(IEnumerable<IPAddress> ipAddresses)
    {
        await using var ctx = await context.CreateDbContextAsync();
        ctx.UpdateRange(ipAddresses);
        await ctx.SaveChangesAsync();
    }
}