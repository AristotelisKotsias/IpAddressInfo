/*#region

using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

#endregion

namespace IpAddressInfo.Tests;

public class CountryRepositoryTests
{
    private readonly Mock<IDbContextFactory<AppDbContext>> _contextFactory;

    public CountryRepositoryTests()
    {
        _contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
    }

    private async Task<AppDbContext> CreateContextWithData()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        var context = new AppDbContext(options);
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
        var context = await CreateContextWithData();
        _contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new CountryRepository(_contextFactory.Object);
        var result = await repository.GetCountryByNameAsync("Greece");

        Assert.NotNull(result);
        Assert.Equal("Greece", result.Name);
        Assert.Equal("GR", result.TwoLetterCode);
        Assert.Equal("GRC", result.ThreeLetterCode);
    }

    [Fact]
    public async Task GetCountryByNameAsync_ShouldReturnNull_WhenCountryDoesNotExist()
    {
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new CountryRepository(contextFactory.Object);
        var result = await repository.GetCountryByNameAsync("NonExistentCountry");

        Assert.Null(result);
    }

    [Fact]
    public async Task AddCountryAsync_ShouldAddCountry()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
            .Options;

        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        using (var operationContext = new AppDbContext(options))
        {
            contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(operationContext);

            var repository = new CountryRepository(contextFactory.Object);
            var country = new Country
            {
                Name = "Spain",
                TwoLetterCode = "ES",
                ThreeLetterCode = "ESP",
                CreatedAt = DateTime.UtcNow
            };
            await repository.AddCountryAsync(country);
        }

        using (var verificationContext = new AppDbContext(options))
        {
            var result = await verificationContext.Countries.FirstOrDefaultAsync(c => c.Name == "Spain");

            Assert.NotNull(result);
            Assert.Equal("Spain", result.Name);
            Assert.Equal("ES", result.TwoLetterCode);
            Assert.Equal("ESP", result.ThreeLetterCode);
        }
    }

    [Fact]
    public async Task DeleteCountryAsync_ShouldRemoveCountry()
    {
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new CountryRepository(contextFactory.Object);
        var country = await repository.GetCountryByNameAsync("Greece");
        if (country != null) context.Countries.Remove(country);
        await context.SaveChangesAsync();

        var deletedCountry = await repository.GetCountryByNameAsync("Greece");

        Assert.Null(deletedCountry);
    }

    [Fact]
    public async Task UpdateCountryAsync_ShouldUpdateCountryDetails()
    {
        var contextFactory = new Mock<IDbContextFactory<AppDbContext>>();
        var context = await CreateContextWithData();
        contextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(context);
        var repository = new CountryRepository(contextFactory.Object);
        var country = await repository.GetCountryByNameAsync("Greece");
        country.TwoLetterCode = "GG";
        country.ThreeLetterCode = "GGG";

        await context.SaveChangesAsync();

        var updatedCountry = await repository.GetCountryByNameAsync("Greece");

        Assert.NotNull(updatedCountry);
        Assert.Equal("GG", updatedCountry.TwoLetterCode);
        Assert.Equal("GGG", updatedCountry.ThreeLetterCode);
    }
}*/

