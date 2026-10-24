using GymManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasColumnType("varchar")
                   .HasMaxLength(100);

            builder.Property(p => p.Description)
                   .HasMaxLength(250);

            builder.Property(p => p.Price)
                   .HasPrecision(10, 2);

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("PlanDurationCheck", "DurationDays between 1 and 365");
            });

            builder.Property(p => p.UpdatedAt)
                    .IsRequired(false);
        }
    }
}
