using LoginMVC.Data;
using LoginMVC.Data.Seeders;
using Microsoft.EntityFrameworkCore;

namespace LoginMVC.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task ApplyMigrationsAsync(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var service = scope.ServiceProvider;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();
            var context = service.GetRequiredService<ApplicationDbContext>();
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "Migration Failure");
            }
        }
    }

    public static async Task ApplySeedingAsync(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var service = scope.ServiceProvider;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();
            var adminUserSeeder = service.GetRequiredService<AdminUserSeeder>();
            var productsDataSeeder = service.GetRequiredService<ProductsDataSeeder>();
            try
            {
                await adminUserSeeder.SeedAsync();
                await productsDataSeeder.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "Admin Seeding failed");
            }
        }
    }
}
