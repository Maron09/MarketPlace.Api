using Marketplace.Modules.Cart.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Marketplace.Modules.Cart.Infrastructure;

internal sealed class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems", schema: "cart");

        builder.HasKey(ci => ci.Id);
        
        builder.Property(ci => ci.Id)
            .ValueGeneratedNever();


        builder.Property(ci => ci.ProductId)
            .IsRequired();

        builder.Property(ci => ci.ProductNameSnapshot)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ci => ci.UnitPriceSnapshot)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(ci => ci.Quantity)
            .IsRequired();
    }
}