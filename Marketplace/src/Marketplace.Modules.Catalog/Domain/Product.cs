using Marketplace.SharedKernel;

namespace Marketplace.Modules.Catalog.Domain
{
    internal sealed class Product
    {
        public Guid Id { get; private set; }
        public Guid VendorId { get; private set; }
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime  UpdatedAtUtc { get; private set; }

        private Product() {  }

        private Product(Guid id, Guid vendorId, string name, string? description, decimal price)
        {
            Id = id;
            VendorId = vendorId;
            Name = name;
            Description = description;
            Price = price;
            IsActive = true;
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public static Product Create(Guid vendorId, string name, string? description, decimal price)
            => new(Guid.NewGuid(), vendorId, name.Trim(), description?.Trim(), price);

        public Result UpdateDetails(string name, string? description, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure("Product name is required.", ErrorType.Validation);

            if (price <= 0)
                return Result.Failure("Price must be greater than zero.", ErrorType.Validation);

            Name = name.Trim();
            Description = description?.Trim();
            Price = price;
            UpdatedAtUtc = DateTime.UtcNow;
            return Result.Success();
        }

        public Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure("Product is already inactive.", ErrorType.Conflict);

            IsActive = false;
            UpdatedAtUtc = DateTime.UtcNow;
            return Result.Success();
        }

        public Result Reactivate()
        {
            if (IsActive)
                return Result.Failure("Product is already active.", ErrorType.Conflict);

            IsActive = true;
            UpdatedAtUtc = DateTime.UtcNow;
            return Result.Success();
        }
    }
}