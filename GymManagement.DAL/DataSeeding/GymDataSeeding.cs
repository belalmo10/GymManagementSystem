using GymManagement.DAL.Data;
using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GymManagement.DAL.DataSeeding
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext context , string folderPath , ILogger logger , CancellationToken ct = default)
        {
            try
            {
                if ( !await context.Plans.AnyAsync(ct) )
                {
                    var plans = LoadDataFromJsonFile<Plan>(folderPath , "plans.json");
                    if ( plans.Any() )
                    {
                        context.Plans.AddRange(plans);
                        logger.LogInformation($"Plans Seeded With Count {plans.Count}");
                    }

                    if ( context.ChangeTracker.HasChanges() )
                        await context.SaveChangesAsync();
                    else
                        logger.LogInformation("Data Already Seeded");
                }
            }
            catch ( Exception ex )
            {
                logger.LogError(ex , "Gym Data Seeded Failed");
                throw;
            }
        }

        private static List<T> LoadDataFromJsonFile<T>(string folderPath , string fileName)
        {
            var filePath = Path.Combine(folderPath , fileName);
            if ( !File.Exists(filePath) )
                throw new FileNotFoundException($"Seed data File Not Found : {filePath}");

            var data = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true ,
            };
            return JsonSerializer.Deserialize<List<T>>(data , options) ?? [];
        }
    }
}
