using Marketplace.Modules.Orders.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Marketplace.Modules.Orders.Infrastructure;


internal sealed class IdempotencyKeyEntityConfiguration : IEntityTypeConfiguration<IdempotencyKey>
{
    public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
    {
        builder.ToTable("IdempotencyKeys", schema: "orders");

        builder.HasKey(x => x.Key);
        builder.Property(x => x.Key).ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.FailureMessage).HasMaxLength(500);

        builder.HasIndex(x => x.UserId);
    }
}