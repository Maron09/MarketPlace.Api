using Marketplace.Modules.Cart.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Modules.Cart.Infrastructure;

internal sealed class CartEntityConfiguration : IEntityTypeConfiguration<Domain.Cart>
{
    public void Configure(EntityTypeBuilder<Domain.Cart> builder)
    {
        builder.ToTable("Carts", schema: "cart");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired();
        
        builder.HasIndex(c => c.UserId)
            .IsUnique();
        
        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();
        
        builder.Property(c => c.UpdatedAtUtc)
            .IsRequired();
        
        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Metadata.FindNavigation(nameof(Domain.Cart.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}