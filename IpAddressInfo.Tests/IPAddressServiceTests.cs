#region

using IpAddressInfo.Dtos;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using IpAddressInfo.Services;
using Microsoft.Extensions.Logging;
using Moq;

#endregion

/*namespace IpAddressInfo.Tests;

public class IPAddressServiceTests
{
    private readonly Mock<ICache> _cacheMock;
    private readonly Mock<ICountryRepository> _countryRepositoryMock;
    private readonly Mock<IExternalIpService> _externalIPServiceMock;
    private readonly IpAddressService _ipAddressService;
    private readonly Mock<IIpRepository> _ipRepositoryMock;
    private readonly Mock<ILogger<IpAddressService>> _loggerMock;

    public IPAddressServiceTests()
    {
        _ipRepositoryMock = new Mock<IIpRepository>();
        _countryRepositoryMock = new Mock<ICountryRepository>();
        _cacheMock = new Mock<ICache>();
        _externalIPServiceMock = new Mock<IExternalIpService>();
        _loggerMock = new Mock<ILogger<IpAddressService>>();
        _ipAddressService = new IpAddressService(
            _ipRepositoryMock.Object,
            _cacheMock.Object,
            _externalIPServiceMock.Object,
            _countryRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetIPAddressDetailsAsync_WhenCacheIsHit_ShouldReturnCachedResult()
    {
        const string ip = "44.255.255.254";
        var cachedIPInfo = new IpAddressDto
            { IP = ip, CountryName = "Greece", TwoLetterCode = "GR", ThreeLetterCode = "GRC" };
        object cacheEntry = cachedIPInfo;
        _cacheMock.Setup(m => m.TryGetValue(ip, out cacheEntry)).Returns(true);
        var result = await _ipAddressService.GetIpAddressDetailsAsync(ip);

        Assert.NotNull(result);
        Assert.Equal(ip, result.IP);
        _ipRepositoryMock.Verify(x => x.GetIpAddressByIpAsync(It.IsAny<string>()), Times.Never);
        _externalIPServiceMock.Verify(x => x.FetchIpAddressDetailsAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetIPAddressDetailsAsync_WhenCacheIsMissed_ShouldReturnDatabaseResult()
    {
        var ip = "44.255.255.254";
        IpAddressDto cachedIPInfo = null;

        object cacheEntry = cachedIPInfo;
        _cacheMock.Setup(m => m.TryGetValue(ip, out cacheEntry)).Returns(false);
        var ipAddress = new IPAddress
        {
            IP = ip,
            Country = new Country
            {
                Name = "Greece",
                TwoLetterCode = "GR",
                ThreeLetterCode = "GRC"
            }
        };
        _ipRepositoryMock.Setup(repo => repo.GetIpAddressByIpAsync(ip)).ReturnsAsync(ipAddress);
        var result = await _ipAddressService.GetIpAddressDetailsAsync(ip);

        Assert.NotNull(result);
        Assert.Equal(ip, result.IP);
        Assert.Equal("Greece", result.CountryName);
        Assert.Equal("GR", result.TwoLetterCode);
        Assert.Equal("GRC", result.ThreeLetterCode);
        _ipRepositoryMock.Verify(x => x.GetIpAddressByIpAsync(ip), Times.Once);
        _externalIPServiceMock.Verify(x => x.FetchIpAddressDetailsAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetIPAddressDetailsAsync_WhenCacheAndDatabaseAreMissed_ShouldReturnExternalServiceResult()
    {
        const string ip = "44.255.255.254";
        IpAddressDto cachedIPInfo = null;
        object cacheEntry = cachedIPInfo;
        _cacheMock.Setup(m => m.TryGetValue(ip, out cacheEntry)).Returns(false);
        _ipRepositoryMock.Setup(repo => repo.GetIpAddressByIpAsync(ip)).ReturnsAsync((IPAddress)null);
        var rawResponse = "1;GR;GRC;Greece";
        _externalIPServiceMock.Setup(service => service.FetchIpAddressDetailsAsync(ip)).ReturnsAsync(rawResponse);
        var result = await _ipAddressService.GetIpAddressDetailsAsync(ip);

        Assert.NotNull(result);
        Assert.Equal(ip, result.IP);
        Assert.Equal("Greece", result.CountryName);
        Assert.Equal("GR", result.TwoLetterCode);
        Assert.Equal("GRC", result.ThreeLetterCode);
        _ipRepositoryMock.Verify(x => x.GetIpAddressByIpAsync(ip), Times.Once);
        _externalIPServiceMock.Verify(x => x.FetchIpAddressDetailsAsync(ip), Times.Once);
    }

    [Fact]
    public async Task GetIPAddressDetailsAsync_WhenExternalServiceFails_ShouldReturnNull()
    {
        const string ip = "44.255.255.254";
        IpAddressDto cachedIPInfo = null;

        object cacheEntry = cachedIPInfo;
        _cacheMock.Setup(m => m.TryGetValue(ip, out cacheEntry)).Returns(false);
        _ipRepositoryMock.Setup(repo => repo.GetIpAddressByIpAsync(ip)).ReturnsAsync((IPAddress)null);
        _externalIPServiceMock.Setup(service => service.FetchIpAddressDetailsAsync(ip)).ReturnsAsync((string)null);
        var result = await _ipAddressService.GetIpAddressDetailsAsync(ip);

        Assert.Null(result);
        _ipRepositoryMock.Verify(x => x.GetIpAddressByIpAsync(ip), Times.Once);
        _externalIPServiceMock.Verify(x => x.FetchIpAddressDetailsAsync(ip), Times.Once);
    }

    [Fact]
    public async Task SaveIpAddressToDatabaseAsync_WhenCountryExists_ShouldAddIpAddress()
    {
        var ipInfo = new IpAddressDto
        {
            IP = "44.255.255.254",
            CountryName = "Greece",
            TwoLetterCode = "GR",
            ThreeLetterCode = "GRC"
        };

        var country = new Country
        {
            Id = 1,
            Name = "Greece",
            TwoLetterCode = "GR",
            ThreeLetterCode = "GRC"
        };

        _countryRepositoryMock.Setup(repo => repo.GetCountryByNameAsync(ipInfo.CountryName)).ReturnsAsync(country);
        await _ipAddressService.SaveIpAddressToDatabaseAsync(ipInfo);
        _countryRepositoryMock.Verify(x => x.AddCountryAsync(It.IsAny<Country>()), Times.Never);
        _ipRepositoryMock.Verify(x => x.AddIpAddressAsync(It.Is<IPAddress>(ip =>
            ip.IP == ipInfo.IP &&
            ip.CountryId == country.Id
        )), Times.Once);
    }

    [Fact]
    public async Task SaveIpAddressToDatabaseAsync_WhenCountryDoesNotExist_ShouldAddCountryAndIpAddress()
    {
        var ipInfo = new IpAddressDto
        {
            IP = "44.255.255.254",
            CountryName = "Greece",
            TwoLetterCode = "GR",
            ThreeLetterCode = "GRC"
        };

        _countryRepositoryMock.Setup(repo => repo.GetCountryByNameAsync(ipInfo.CountryName))
            .ReturnsAsync((Country)null);

        await _ipAddressService.SaveIpAddressToDatabaseAsync(ipInfo);
        _countryRepositoryMock.Verify(x => x.AddCountryAsync(It.IsAny<Country>()), Times.Once);
        _ipRepositoryMock.Verify(x => x.AddIpAddressAsync(It.IsAny<IPAddress>()), Times.Once);
    }
}*/