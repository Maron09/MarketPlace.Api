using Marketplace.Modules.Vendors.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Marketplace.Modules.Vendors.Infrastructure
{
    internal sealed class VendorEntityConfiguration : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {
            builder.ToTable("Vendors", schema: "vendors");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.UserId)
                .IsRequired();

            builder.HasIndex(v => v.UserId)
                .IsUnique();

            builder.Property(v => v.BusinessName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(v => v.Description)
                .HasMaxLength(2000);

            builder.Property(v => v.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(v => v.CreatedAtUtc)
                .IsRequired();

            builder.Property(v => v.ReviewedAtUtc);
            builder.Property(v => v.ReviewedBy);
        }
    }
}