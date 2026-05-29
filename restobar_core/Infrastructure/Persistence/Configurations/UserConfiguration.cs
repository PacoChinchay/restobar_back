using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using restobar_core.Domain.Entities;

namespace restobar_core.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnType("varchar(50)")
            .ValueGeneratedNever();

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Initials)
            .IsRequired()
            .HasMaxLength(4);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(u => u.PinHash)
            .IsRequired()
            .HasColumnName("pin_hash");

        builder.Property(u => u.MonthlySalary)
            .HasColumnType("decimal(10,2)");
    }
}
