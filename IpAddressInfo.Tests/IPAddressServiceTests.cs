using IpAddressInfo.Dtos;
using IpAddressInfo.Entities;
using IpAddressInfo.Interfaces;
using IpAddressInfo.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace IpAddressInfo.Tests;

public class IPAddressServiceTests
{
    private readonly Mock<IIPRepository> _ipRepositoryMock;
    private readonly Mock<ICountryRepository> _countryRepositoryMock;
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly Mock<IExternalIPService> _externalIPServiceMock;
    private readonly Mock<ILogger<IPAddressService>> _loggerMock;
    private readonly IPAddressService _ipAddressService;

    public IPAddressServiceTests()
    {
        _ipRepositoryMock = new Mock<IIPRepository>();
        _countryRepositoryMock = new Mock<ICountryRepository>();
        _cacheMock = new Mock<IMemoryCache>();
        _externalIPServiceMock = new Mock<IExternalIPService>();
        _loggerMock = new Mock<ILogger<IPAddressService>>();
        _ipAddressService = new IPAddressService(
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
        var cachedIPInfo = new IPAddressDto { IP = ip, CountryName = "Greece", TwoLetterCode = "GR", ThreeLetterCode = "GRC" };

        object cacheEntry = cachedIPInfo;
        _cacheMock.Setup(m => m.TryGetValue(ip, out cacheEntry)).Returns(true);

        var result = await _ipAddressService.GetIPAddressDetailsAsync(ip);

        Assert.NotNull(result);
        Assert.Equal(ip, result.IP);
        _ipRepositoryMock.Verify(x => x.GetIPAddressByIPAsync(It.IsAny<string>()), Times.Never);
        _externalIPServiceMock.Verify(x => x.FetchIPAddressDetailsAsync(It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public async Task GetIPAddressDetailsAsync_WhenCacheIsMissed_ShouldReturnDatabaseResult()
    {
        var ip = "44.255.255.254";
        IPAddressDto cachedIPInfo = null;

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
        var ipRepositoryMock = new Mock<IIPRepository>();
        ipRepositoryMock.Setup(repo => repo.GetIPAddressByIPAsync(ip))
            .ReturnsAsync(
                new IPAddress
                {
                    IP = "44.255.255.254",
                    Country = new Country
                    {
                        Name = "Greece",
                        TwoLetterCode = "GR",
                        ThreeLetterCode = "GRC"
                    }
                }
            );
        //_ipRepositoryMock.Setup(repo => repo.GetIPAddressByIPAsync(ip)).ReturnsAsync(ipAddress);

        var result = await _ipAddressService.GetIPAddressDetailsAsync(ip);
        
        Assert.NotNull(result);
        Assert.Equal(ip, result.IP);
        Assert.Equal("Greece", result.CountryName);
        Assert.Equal("GR", result.TwoLetterCode);
        Assert.Equal("GRC", result.ThreeLetterCode);
        _ipRepositoryMock.Verify(x => x.GetIPAddressByIPAsync(ip), Times.Once);
        _externalIPServiceMock.Verify(x => x.FetchIPAddressDetailsAsync(It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public async Task GetIPAddressDetailsAsync_WhenCacheAndDatabaseAreMissed_ShouldReturnExternalServiceResult()
    {
        const string ip = "44.255.255.254";
        IPAddressDto cachedIPInfo = null;

        object cacheEntry = cachedIPInfo;
        _cacheMock.Setup(m => m.TryGetValue(ip, out cacheEntry)).Returns(false);

        _ipRepositoryMock.Setup(repo => repo.GetIPAddressByIPAsync(ip)).ReturnsAsync((IPAddress)null);

        var rawResponse = "1;GR;GRC;Greece";
        _externalIPServiceMock.Setup(service => service.FetchIPAddressDetailsAsync(ip)).ReturnsAsync(rawResponse);

        var result = await _ipAddressService.GetIPAddressDetailsAsync(ip);

        Assert.NotNull(result);
        Assert.Equal(ip, result.IP);
        Assert.Equal("Greece", result.CountryName);
        Assert.Equal("GR", result.TwoLetterCode);
        Assert.Equal("GRC", result.ThreeLetterCode);
        _ipRepositoryMock.Verify(x => x.GetIPAddressByIPAsync(ip), Times.Once);
        _externalIPServiceMock.Verify(x => x.FetchIPAddressDetailsAsync(ip), Times.Once);
    }

}