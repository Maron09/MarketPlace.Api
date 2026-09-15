using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Catalog.Application;
using Marketplace.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Modules.Catalog.Infrastructure
{
    internal sealed class CategoryRepository : ICategoryRepository
    {
        private readonly MarketplaceDbContext _dbContext;

        public CategoryRepository(MarketplaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken)
            => await _dbContext.Set<Category>().FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
        
        public async Task<bool> NameExistsAsync(string name, Guid? excludingCategoryId, CancellationToken cancellationToken)
            => await _dbContext.Set<Category>()
                .AnyAsync(c => c.Name == name && c.Id != excludingCategoryId, cancellationToken);
        
        public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken)
            => await _dbContext.Set<Category>().OrderBy(c => c.Name).ToListAsync(cancellationToken);
        
        public async Task AddAsync(Category category, CancellationToken cancellationToken)
            => await _dbContext.Set<Category>().AddAsync(category, cancellationToken);
        
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _dbContext.SaveChangesAsync(cancellationToken);
    }
}