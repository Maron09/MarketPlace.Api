using Marketplace.Modules.Inventory.Domain;

namespace Marketplace.Modules.Inventory.Application;

internal interface IStockItemRepository
{
    Task<StockItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);
    Task AddAsync(StockItem stockItem, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}