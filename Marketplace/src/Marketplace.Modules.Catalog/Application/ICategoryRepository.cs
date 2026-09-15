using Marketplace.Modules.Catalog.Domain;



namespace Marketplace.Modules.Catalog.Application
{
    internal interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken);
        Task<bool> NameExistsAsync(string name, Guid? excludingCategoryId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken);
        Task AddAsync(Category category, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}