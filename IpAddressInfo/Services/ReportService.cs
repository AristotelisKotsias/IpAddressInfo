using System.Data;
using Dapper;
using IpAddressInfo.Dtos;
using IpAddressInfo.Interfaces;
using Npgsql;

namespace IpAddressInfo.Services
{
    public class ReportService : IReportService
    {
        private readonly string _connectionString;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IConfiguration configuration, ILogger<ReportService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<IEnumerable<CountryReportDto>> GetCountryReportAsync(IEnumerable<string>? countryCodes)
        {
            try
            {
                using IDbConnection db = new NpgsqlConnection(_connectionString);
                var sql = @"
                    SELECT
                        c.""Name"" AS CountryName,
                        COUNT(ip.""Id"") AS AddressesCount,
                        MAX(ip.""UpdatedAt"") AS LastAddressUpdated
                    FROM
                        public.""Countries"" c
                    JOIN
                        public.""IPAddresses"" ip ON c.""Id"" = ip.""CountryId""";

                if (countryCodes != null)
                {
                    sql += " WHERE c.\"TwoLetterCode\" = ANY(@CountryCodes)";
                }

                sql += " GROUP BY c.\"Name\"";

                var result = await db.QueryAsync<CountryReportDto>(sql, new { CountryCodes = countryCodes?.ToArray() });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the country report.");
                return new List<CountryReportDto>();
            }
        }
    }
}