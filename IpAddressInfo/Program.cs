#region

using IpAddressInfo.Data;
using IpAddressInfo.Interfaces;
using IpAddressInfo.Repositories;
using IpAddressInfo.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IIpRepository, IpRepository>();
builder.Services.AddScoped<ICache, MemoryCache>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IIpAddressService, IpAddressService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddHttpClient<IExternalIpService, ExternalIpService>();
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
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();
app.Run();