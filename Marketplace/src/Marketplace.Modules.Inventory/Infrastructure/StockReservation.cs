using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Inventory.Contracts;
using Marketplace.Modules.Inventory.Domain;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Modules.Inventory.Infrastructure;

internal sealed class StockReservation : IStockReservation, IStockRelease, IStockAvailabilityReader
{
    private readonly MarketplaceDbContext _dbContext;

    public StockReservation(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> TryReserveAsync(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var stockItem = await _dbContext.Set<StockItem>()
            .FirstOrDefaultAsync(s => s.ProductId ==productId, cancellationToken);
        
        if (stockItem is null)
        {
            return false;
        }
        var result = stockItem.Reserve(quantity);
        return result.IsSuccess;
    }

    public async Task ReleaseAsync(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var stockItem = await _dbContext.Set<StockItem>()
            .FirstOrDefaultAsync(s => s.ProductId == productId, cancellationToken);

        stockItem?.Release(quantity);
    }

    public async Task<int> GetAvailableQuantityAsync(Guid productId, CancellationToken cancellationToken)
    {
        var stockItem = await _dbContext.Set<StockItem>()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ProductId == productId, cancellationToken);

        return stockItem?.QuantityOnHand ?? 0;
    }
}