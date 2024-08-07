using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IpAddressInfo.Tests;

public class CountryRepositoryTests
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public CountryRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
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

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task GetCountryByNameAsync_ShouldReturnCountry_WhenCountryExists()
        {
            await using var context = await CreateContextWithData();
            var repository = new CountryRepository(context);

            var result = await repository.GetCountryByNameAsync("Greece");

            Assert.NotNull(result);
            Assert.Equal("Greece", result.Name);
            Assert.Equal("GR", result.TwoLetterCode);
            Assert.Equal("GRC", result.ThreeLetterCode);
        }

        [Fact]
        public async Task GetCountryByNameAsync_ShouldReturnNull_WhenCountryDoesNotExist()
        {
            await using var context = await CreateContextWithData();
            var repository = new CountryRepository(context);

            var result = await repository.GetCountryByNameAsync("NonExistentCountry");

            Assert.Null(result);
        }

        [Fact]
        public async Task AddCountryAsync_ShouldAddCountry()
        {
            await using var context = new AppDbContext(_options);
            var repository = new CountryRepository(context);

            var country = new Country
            {
                Name = "Spain",
                TwoLetterCode = "ES",
                ThreeLetterCode = "ESP",
                CreatedAt = DateTime.UtcNow
            };

            await repository.AddCountryAsync(country);
            var result = await context.Countries.FirstOrDefaultAsync(c => c.Name == "Spain");
            
            Assert.NotNull(result);
            Assert.Equal("Spain", result.Name);
            Assert.Equal("ES", result.TwoLetterCode);
            Assert.Equal("ESP", result.ThreeLetterCode);
        }
    }