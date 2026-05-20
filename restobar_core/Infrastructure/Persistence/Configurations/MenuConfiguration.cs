using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using restobar_core.Domain.Entities;

namespace restobar_core.Infrastructure.Persistence.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("menus");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
        builder.Property(m => m.IsActive).IsRequired();
        builder.Property(m => m.CreatedAt).IsRequired();
        builder.HasMany(m => m.Items)
            .WithOne(i => i.Menu)
            .HasForeignKey(i => i.MenuId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
