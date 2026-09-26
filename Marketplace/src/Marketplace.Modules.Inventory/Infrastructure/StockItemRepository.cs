using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Inventory.Application;
using Marketplace.Modules.Inventory.Domain;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Modules.Inventory.Infrastructure;

internal sealed class StockItemRepository : IStockItemRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public StockItemRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StockItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
        => await _dbContext.Set<StockItem>().FirstOrDefaultAsync(s => s.ProductId == productId, cancellationToken);
    
    public async Task AddAsync(StockItem stockItem, CancellationToken cancellationToken)
        => await _dbContext.Set<StockItem>().AddAsync(stockItem,cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}