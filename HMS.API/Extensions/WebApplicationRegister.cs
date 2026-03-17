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
    }
}
