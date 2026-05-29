using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using restobar_core.Domain.Entities;

namespace restobar_core.Infrastructure.Persistence.Configurations;

public class CashMovementConfiguration : IEntityTypeConfiguration<CashMovement>
{
    public void Configure(EntityTypeBuilder<CashMovement> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Amount).HasColumnType("decimal(10,2)");
        builder.Property(m => m.MovementType).HasMaxLength(20);
        builder.Property(m => m.Description).HasMaxLength(255);
        builder.Property(m => m.CreatedBy).HasMaxLength(100);
        builder.HasOne(m => m.Session)
               .WithMany(s => s.Movements)
               .HasForeignKey(m => m.CashSessionId);
    }
}
