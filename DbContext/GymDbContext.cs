namespace GymManagementSystem.DbContext;
using GymManagementSystem.Configurations;
using GymManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

public class GymDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Database=GymManagementSystem;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PlanConfiguration());
    }

    public DbSet<Plan> Plans{ get; set; }
}
