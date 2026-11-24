namespace GymManagement.DAL.Data;

using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

public class GymDbContext : IdentityDbContext<ApplicationUser>
{
    public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
    {

    }
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Database=GymManagementSystem;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True");
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfiguration(new PlanConfiguration());
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


        modelBuilder.Entity<ApplicationUser>(eb =>
        {
            eb.Property(u => u.FirestName)
               .HasColumnType("varchar")
               .HasMaxLength(50);

            eb.Property(u => u.LastName)
              .HasColumnType("varchar")
              .HasMaxLength(50);

        });
    }

    public DbSet<Plan> Plans { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<HealthRecord> HealthRecords { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<MemberShip> MemberShips { get; set; }
    public DbSet<Booking> Bookings { get; set; }

}
