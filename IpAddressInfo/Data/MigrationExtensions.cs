using Microsoft.EntityFrameworkCore;

namespace IpAddressInfo.Data;

public static class ApplicationBuilderExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        
        using var context = dbContextFactory.CreateDbContext();
        context.Database.Migrate();
    }
}