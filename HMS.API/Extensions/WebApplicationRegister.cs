using HMS.Core.Contracts;
using HMS.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HMS.API.Extensions
{
    public static class WebApplicationRegister
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var hotelDbContext = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
            var pendingMigrations = await hotelDbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
                await hotelDbContext.Database.MigrateAsync();

            return app;
        }
        public static async Task<WebApplication> SeedIdentityDataAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            var identityDataIntializaer = scope.ServiceProvider.GetRequiredService<IDataIntializer>();

            await identityDataIntializaer.IntializeAdminAndRoleAsync();

            return app;
        }
    }
}
