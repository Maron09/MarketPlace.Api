using Marketplace.Modules.Orders.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Marketplace.Modules.Orders.Infrastructure;

internal sealed class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems", schema: "orders");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id)
            .ValueGeneratedNever();

        builder.Property(oi => oi.ProductId)
            .IsRequired();

        builder.Property(oi => oi.VendorId)
            .IsRequired();

        builder.HasIndex(oi => oi.VendorId);

        builder.Property(oi => oi.ProductNameSnapshot)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(oi => oi.UnitPriceSnapshot)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .IsRequired();
    }
}