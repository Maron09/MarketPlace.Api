using Marketplace.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Marketplace.Modules.Catalog.Infrastructure
{
    internal sealed class ProductImageEntityConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImages", schema: "catalog");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.StorageKey)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(pi => pi.DisplayOrder)
                .IsRequired();
            
            builder.Property(pi => pi.CreatedAtUtc)
                .IsRequired();

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pi => pi.ProductId);
        }
    }
}