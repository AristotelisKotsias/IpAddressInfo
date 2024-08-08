#region

using IpAddressInfo.Data;
using IpAddressInfo.Entities;
using IpAddressInfo.Repositories;
using Microsoft.EntityFrameworkCore;

#endregion

namespace IpAddressInfo.Tests;

public class CountryRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _options;

    public CountryRepositoryTests()
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
        await using var context = await CreateContextWithData();
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
    
    [Fact]
    public async Task AddCountryAsync_ShouldThrowException_WhenCountryIsNull()
    {
        await using var context = await CreateContextWithData();
        var repository = new CountryRepository(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddCountryAsync(null));
    }
    
    [Fact]
    public async Task AddCountryAsync_ShouldNotAddCountry_WhenCountryWithSameNameExists()
    {
        await using var context = await CreateContextWithData();
        var repository = new CountryRepository(context);

        var country = new Country
        {
            Name = "Greece",
            TwoLetterCode = "GR",
            ThreeLetterCode = "GRC",
            CreatedAt = DateTime.UtcNow
        };

        await Assert.ThrowsAsync<DbUpdateException>(() => repository.AddCountryAsync(country));
    }
    
    [Fact]
    public async Task DeleteCountryAsync_ShouldRemoveCountry()
    {
        await using var context = await CreateContextWithData();
        var repository = new CountryRepository(context);

        var country = await repository.GetCountryByNameAsync("Greece");
        context.Countries.Remove(country);
        await context.SaveChangesAsync();

        var deletedCountry = await repository.GetCountryByNameAsync("Greece");

        Assert.Null(deletedCountry);
    }
    
    [Fact]
    public async Task UpdateCountryAsync_ShouldUpdateCountryDetails()
    {
        await using var context = await CreateContextWithData();
        var repository = new CountryRepository(context);

        var country = await repository.GetCountryByNameAsync("Greece");
        country.TwoLetterCode = "GG";
        country.ThreeLetterCode = "GGG";

        await context.SaveChangesAsync();

        var updatedCountry = await repository.GetCountryByNameAsync("Greece");

        Assert.NotNull(updatedCountry);
        Assert.Equal("GG", updatedCountry.TwoLetterCode);
        Assert.Equal("GGG", updatedCountry.ThreeLetterCode);
    }
}