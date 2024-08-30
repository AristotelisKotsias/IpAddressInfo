
using IpAddressInfo.Data;
using IpAddressInfo.Interfaces;
using IpAddressInfo.Repositories;
using IpAddressInfo.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("Ip2cService", client =>
{
    var baseUrl = builder.Configuration["Ip2cService:BaseUrl"];
    if (baseUrl != null) client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisCache:Configuration"];
    options.InstanceName = builder.Configuration["RedisCache:IpInfoInstance"];
});
builder.Services.AddMemoryCache();
builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IIpRepository, IpRepository>();
builder.Services.AddSingleton<ICache, MemoryCache>();
builder.Services.AddTransient<ICountryRepository, CountryRepository>();
builder.Services.AddTransient<IIpAddressService, IpAddressService>();
builder.Services.AddTransient<IReportService, ReportService>();
builder.Services.AddHttpClient("Ip2cService");
builder.Services.AddTransient<IExternalIpService, ExternalIpService>();
//builder.Services.AddHostedService<IpUpdateService>();

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