using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using restobar_core.Domain.Entities;

namespace restobar_core.Infrastructure.Persistence.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ProductName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.UnitPrice)
            .IsRequired()
            .HasColumnType("numeric(10,2)");

        builder.Property(s => s.Quantity)
            .IsRequired();

        builder.Property(s => s.PaymentMethod)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(s => s.Total)
            .IsRequired()
            .HasColumnType("numeric(10,2)");

        builder.Property(s => s.RegisteredAt)
            .IsRequired()
            .HasColumnType("timestamptz");

        builder.Property(s => s.RegisteredBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
