#region

using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Repositories;
using Microsoft.EntityFrameworkCore;

#endregion

namespace IpAddressInfo.Tests;

public class IPRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _options;

    public IPRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
    }

    private async Task<AppDbContext> CreateContextWithData()
    {
        var context = new AppDbContext(_options);

        var country = new Country
        {
            Name = "Greece",
            TwoLetterCode = "GR",
            ThreeLetterCode = "GRC",
            CreatedAt = DateTime.UtcNow
        };

        var ipAddress = new IPAddress
        {
            IP = "44.255.255.254",
            Country = country,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Countries.Add(country);
        context.IPAddresses.Add(ipAddress);

        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async Task GetIPAddressByIPAsync_ShouldReturnIPAddress_WhenIPAddressExists()
    {
        await using var context = await CreateContextWithData();
        var repository = new IpRepository(context);

        var result = await repository.GetIpAddressByIpAsync("44.255.255.254");

        Assert.NotNull(result);
        Assert.Equal("44.255.255.254", result.IP);
        Assert.Equal("Greece", result.Country.Name);
    }

    [Fact]
    public async Task GetIPAddressByIPAsync_ShouldReturnNull_WhenIPAddressDoesNotExist()
    {
        await using var context = await CreateContextWithData();
        var repository = new IpRepository(context);

        var result = await repository.GetIpAddressByIpAsync("192.168.0.1");

        Assert.Null(result);
    }

    [Fact]
    public async Task AddIPAddressAsync_ShouldAddIPAddress()
    {
        await using var context = new AppDbContext(_options);
        var repository = new IpRepository(context);

        var country = new Country
        {
            Name = "Greece",
            TwoLetterCode = "GR",
            ThreeLetterCode = "GRC",
            CreatedAt = DateTime.UtcNow
        };

        var ipAddress = new IPAddress
        {
            IP = "44.255.255.254",
            Country = country,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.AddIpAddressAsync(ipAddress);
        var result = await context.IPAddresses
            .Include(ip => ip.Country)
            .FirstOrDefaultAsync(ip => ip.IP == "44.255.255.254");

        Assert.NotNull(result);
        Assert.Equal("44.255.255.254", result.IP);
        Assert.Equal("Greece", result.Country.Name);
    }

    [Fact]
    public async Task GetIPAddressesInBatchAsync_ShouldReturnIPAddresses()
    {
        await using var context = await CreateContextWithData();
        var repository = new IpRepository(context);

        var result = await repository.GetIpAddressesInBatchAsync(0, 10);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("44.255.255.254", result.First().IP);
        Assert.Equal("Greece", result.First().Country.Name);
    }
    
    [Fact]
    public async Task AddIPAddressAsync_ShouldThrowException_WhenIPAddressIsNull()
    {
        await using var context = new AppDbContext(_options);
        var repository = new IpRepository(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddIpAddressAsync(null));
    }
    [Fact]
    public async Task GetIPAddressesInBatchAsync_ShouldReturnMultipleIPAddresses()
    {
        await using var context = await CreateContextWithData();
        var repository = new IpRepository(context);

        var country = new Country
        {
            Name = "Spain",
            TwoLetterCode = "ES",
            ThreeLetterCode = "ESP",
            CreatedAt = DateTime.UtcNow
        };

        var ipAddress1 = new IPAddress
        {
            IP = "123.123.123.123",
            Country = country,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var ipAddress2 = new IPAddress
        {
            IP = "124.124.124.124",
            Country = country,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.IPAddresses.Add(ipAddress1);
        context.IPAddresses.Add(ipAddress2);
        await context.SaveChangesAsync();

        var result = await repository.GetIpAddressesInBatchAsync(0, 10);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }
    
    [Fact]
    public async Task GetIPAddressesInBatchAsync_ShouldReturnEmpty_WhenNoIPAddressesInRange()
    {
        await using var context = new AppDbContext(_options);
        var repository = new IpRepository(context);

        var result = await repository.GetIpAddressesInBatchAsync(10, 10);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetIPAddressesInBatchAsync_ShouldReturnCorrectBatch()
    {
        await using var context = await CreateContextWithData();
        var repository = new IpRepository(context);

        var country = new Country
        {
            Name = "Spain",
            TwoLetterCode = "ES",
            ThreeLetterCode = "ESP",
            CreatedAt = DateTime.UtcNow
        };

        var ipAddress1 = new IPAddress
        {
            IP = "123.123.123.123",
            Country = country,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var ipAddress2 = new IPAddress
        {
            IP = "124.124.124.124",
            Country = country,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.IPAddresses.Add(ipAddress1);
        context.IPAddresses.Add(ipAddress2);
        await context.SaveChangesAsync();

        var result = await repository.GetIpAddressesInBatchAsync(1, 1);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("123.123.123.123", result.First().IP);
    }
    
    [Fact]
    public async Task AddIPAddressAsync_ShouldNotAddDuplicateIP()
    {
        await using var context = await CreateContextWithData();
        var repository = new IpRepository(context);

        var country = new Country
        {
            Name = "Greece",
            TwoLetterCode = "GR",
            ThreeLetterCode = "GRC",
            CreatedAt = DateTime.UtcNow
        };

        var ipAddress = new IPAddress
        {
            IP = "44.255.255.254",
            Country = country,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await Assert.ThrowsAsync<DbUpdateException>(() => repository.AddIpAddressAsync(ipAddress));
    }
}