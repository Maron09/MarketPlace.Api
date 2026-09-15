using Marketplace.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Marketplace.Modules.Catalog.Infrastructure
{
    internal sealed class ProductCategoryEntityConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProductCategories", schema: "catalog");

            builder.HasKey(pc => new { pc.ProductId, pc.CategoryId });

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}