using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IpAddressInfo.Tests;

public class IPRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public IPRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName:  $"TestDb_{Guid.NewGuid()}")
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
            var repository = new IPRepository(context);
            
            var result = await repository.GetIPAddressByIPAsync("44.255.255.254");
            
            Assert.NotNull(result);
            Assert.Equal("44.255.255.254", result.IP);
            Assert.Equal("Greece", result.Country.Name);
        }

        [Fact]
        public async Task GetIPAddressByIPAsync_ShouldReturnNull_WhenIPAddressDoesNotExist()
        {
            await using var context = await CreateContextWithData();
            var repository = new IPRepository(context);
            
            var result = await repository.GetIPAddressByIPAsync("192.168.0.1");

            Assert.Null(result);
        }

        [Fact]
        public async Task AddIPAddressAsync_ShouldAddIPAddress()
        {
            await using var context = new AppDbContext(_options);
            var repository = new IPRepository(context);

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
            
            await repository.AddIPAddressAsync(ipAddress);
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
            var repository = new IPRepository(context);
            
            var result = await repository.GetIPAddressesInBatchAsync(0, 10);
            
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("44.255.255.254", result.First().IP);
            Assert.Equal("Greece", result.First().Country.Name);
        }
    }