namespace Marketplace.Modules.Catalog.Domain
{
    internal sealed class ProductCategory
    {
        public Guid ProductId { get; private set; }
        public Guid CategoryId { get; private set; }

        private ProductCategory() { }

        public ProductCategory(Guid productId, Guid categoryId)
        {
            ProductId = productId;
            CategoryId = categoryId;
        }
    }
}