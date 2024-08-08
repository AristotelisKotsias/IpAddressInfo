#region

using IpAddressInfo.Dtos;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using IpAddressInfo.Services;
using Microsoft.Extensions.Logging;
using Moq;

#endregion

namespace IpAddressInfo.Tests;

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
}