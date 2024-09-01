#region

using IpAddressInfo.Data;
using IpAddressInfo.Interfaces;
using IpAddressInfo.Repositories;
using IpAddressInfo.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;

#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("Ip2cService", client =>
{
    var baseUrl = builder.Configuration["Ip2cService:BaseUrl"];
    if (baseUrl != null) client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddMemoryCache();
builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("redis");
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("redis")!);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
builder.Services.AddTransient<IIpRepository, IpRepository>();
builder.Services.AddSingleton<ICache, MemoryCache>();
builder.Services.AddTransient<ICountryRepository, CountryRepository>();
builder.Services.AddTransient<IIpAddressService, IpAddressService>();
builder.Services.AddTransient<IReportService, ReportService>();
builder.Services.AddHttpClient("Ip2cService");
builder.Services.AddTransient<IExternalIpService, ExternalIpService>();
builder.Services.AddHostedService<IpUpdateService>();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Run();