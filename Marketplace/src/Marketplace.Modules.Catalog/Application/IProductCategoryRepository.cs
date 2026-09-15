namespace Marketplace.Modules.Catalog.Application
{
    internal interface IProductCategoryRepository
    {
        Task<bool> ExistsAsync(Guid productId, Guid categoryId, CancellationToken cancellationToken);
        Task AddAsync(Guid productId, Guid categoryId, CancellationToken cancellationToken);
        Task RemoveAsync(Guid productId, Guid categoryId, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}