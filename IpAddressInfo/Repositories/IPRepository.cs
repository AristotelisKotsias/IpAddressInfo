using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpAddressInfo.Repositories;

public class IPRepository : IIPRepository
{
    private readonly AppDbContext _context;

    public IPRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IPAddress?> GetIPAddressByIPAsync(string ip)
    {
        return await _context.IPAddresses
            .Include(ipAddress => ipAddress.Country)
            .FirstOrDefaultAsync(ipAddress => ipAddress.IP == ip);
    }

    public async Task AddIPAddressAsync(IPAddress ipAddress)
    {
        _context.IPAddresses.Add(ipAddress);
        await _context.SaveChangesAsync();
    }

    public async Task<List<IPAddress>> GetIPAddressesInBatchAsync(int skip, int take)
    {
        return await _context.IPAddresses
            .Include(ipAddress => ipAddress.Country)
            .OrderBy(ipAddress => ipAddress.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}