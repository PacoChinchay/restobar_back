using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using restobar_core.Domain.Entities;

namespace restobar_core.Infrastructure.Persistence.Configurations;

public class CashSessionConfiguration : IEntityTypeConfiguration<CashSession>
{
    public void Configure(EntityTypeBuilder<CashSession> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.InitialAmount).HasColumnType("decimal(10,2)");
        builder.Property(s => s.OpenedBy).HasMaxLength(100);
        builder.HasMany(s => s.Movements)
               .WithOne(m => m.Session)
               .HasForeignKey(m => m.CashSessionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
