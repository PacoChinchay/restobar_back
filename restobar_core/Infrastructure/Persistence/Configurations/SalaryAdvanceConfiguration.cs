using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using restobar_core.Domain.Entities;

namespace restobar_core.Infrastructure.Persistence.Configurations;

public class SalaryAdvanceConfiguration : IEntityTypeConfiguration<SalaryAdvance>
{
    public void Configure(EntityTypeBuilder<SalaryAdvance> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Amount).HasColumnType("decimal(10,2)");
        builder.Property(a => a.UserId).HasMaxLength(50);
        builder.Property(a => a.EmployeeName).HasMaxLength(100);
        builder.Property(a => a.Notes).HasMaxLength(255);
        builder.Property(a => a.RegisteredBy).HasMaxLength(100);
    }
}
