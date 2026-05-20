using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using restobar_core.Domain.Entities;

namespace restobar_core.Infrastructure.Persistence.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("menu_items");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(10,2)");
        builder.Property(i => i.InitialQuantity).IsRequired();
        builder.Property(i => i.RemainingQuantity).IsRequired();
    }
}
