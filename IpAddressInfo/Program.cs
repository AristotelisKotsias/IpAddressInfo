using IpAddressInfo.Data;
using IpAddressInfo.Interfaces;
using IpAddressInfo.Repositories;
using IpAddressInfo.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add memory cache
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IIPRepository, IPRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IIPAddressService, IPAddressService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddHttpClient<IExternalIPService, ExternalIPService>();
builder.Services.AddHostedService<IPUpdateService>();

// Configure Serilog
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
//app.UseAuthorization();
app.MapControllers();
app.Run();