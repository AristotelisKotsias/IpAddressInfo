/*#region

using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

#endregion

namespace IpAddressInfo.Tests;

public class IPRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    public IPRepositoryTests(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;
    }

    private async Task<AppDbContext> CreateContextWithData()
    {
        var context = await _contextFactory.CreateDbContextAsync();
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
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new IpRepository(contextFactory.Object);
        var result = await repository.GetIpAddressByIpAsync("44.255.255.254");

        Assert.NotNull(result);
        Assert.Equal("44.255.255.254", result.IP);
        Assert.Equal("Greece", result.Country.Name);
    }

    [Fact]
    public async Task GetIPAddressByIPAsync_ShouldReturnNull_WhenIPAddressDoesNotExist()
    {
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new IpRepository(contextFactory.Object);
        var result = await repository.GetIpAddressByIpAsync("192.168.0.1");

        Assert.Null(result);
    }

    [Fact]
    public async Task AddIPAddressAsync_ShouldAddIPAddress()
    {
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new IpRepository(contextFactory.Object);
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
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new IpRepository(contextFactory.Object);
        var result = await repository.GetIpAddressesInBatchAsync(0, 10);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("44.255.255.254", result.First().IP);
        Assert.Equal("Greece", result.First().Country.Name);
    }

    [Fact]
    public async Task AddIPAddressAsync_ShouldThrowException_WhenIPAddressIsNull()
    {
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new IpRepository(contextFactory.Object);
        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddIpAddressAsync(null));
    }
    [Fact]
    public async Task GetIPAddressesInBatchAsync_ShouldReturnMultipleIPAddresses()
    {
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new IpRepository(contextFactory.Object);
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
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new IpRepository(contextFactory.Object);
        var result = await repository.GetIpAddressesInBatchAsync(10, 10);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetIPAddressesInBatchAsync_ShouldReturnCorrectBatch()
    {
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new IpRepository(contextFactory.Object);
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
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new IpRepository(contextFactory.Object);
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
}*/

