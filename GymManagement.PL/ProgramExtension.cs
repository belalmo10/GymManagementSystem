using GymManagement.DAL.Data;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.DataSeeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.PL
{
    public static class ProgramExtension
    {
        public async static Task MigrateAndSeedDatabaseAsync(this WebApplication app)
        {
            using var scoped = app.Services.CreateScope();
            var context = scoped.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scoped.ServiceProvider.GetRequiredService<ILogger<Plan>>();
            var roleManager = scoped.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scoped.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if ( pendingMigrations.Any() )
            {
                logger.LogInformation($"Applying {pendingMigrations.Count()} Pending Migrations");
                await context.Database.MigrateAsync();
            }

            var folderPath = Path.Combine(app.Environment.ContentRootPath , "wwwroot" , "Files");

            await GymDataSeeding.SeedAsync(context , folderPath , logger);
            await IdentityDataSeeding.SeedAsync(roleManager , userManager , logger);
        }
    }
}
