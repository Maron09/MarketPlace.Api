namespace Marketplace.Modules.Catalog.Domain
{
    internal sealed class ProductImage
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public string StorageKey { get; private set; } = null!;
        public int DisplayOrder { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private ProductImage() {  }

        public ProductImage(Guid productId, string storageKey, int displayOrder)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            StorageKey = storageKey;
            DisplayOrder = displayOrder;
            CreatedAtUtc = DateTime.UtcNow;
        }
    }
}