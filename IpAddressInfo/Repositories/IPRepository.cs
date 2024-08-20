#region

using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using Microsoft.EntityFrameworkCore;

#endregion

namespace IpAddressInfo.Repositories;

public class IpRepository : IIpRepository
{
    private readonly AppDbContext _context;

    public IpRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IPAddress?> GetIpAddressByIpAsync(string ip)
    {
        return await _context.IPAddresses
            .Include(ipAddress => ipAddress.Country)
            .FirstOrDefaultAsync(ipAddress => ipAddress.IP == ip);
    }

    public async Task AddIpAddressAsync(IPAddress ipAddress)
    {
        ArgumentNullException.ThrowIfNull(ipAddress);
        var existingIpAddress = await _context.IPAddresses
            .AnyAsync(ip => ip.IP == ipAddress.IP);
        if (existingIpAddress)
            throw new DbUpdateException($"An IP address with the IP '{ipAddress.IP}' already exists.",
                new Exception("Unique constraint violation"));
        _context.IPAddresses.Add(ipAddress);
        await _context.SaveChangesAsync();
    }

    public async Task<List<IPAddress>> GetIpAddressesInBatchAsync(int skip, int take)
    {
        return await _context.IPAddresses
            .Include(ipAddress => ipAddress.Country)
            .OrderBy(ipAddress => ipAddress.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}