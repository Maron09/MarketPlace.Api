using Marketplace.Modules.Catalog.Contracts;
using Marketplace.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Marketplace.Modules.Inventory.Application;

public sealed class InventoryApplicationService
{
    private const int MaxRetries = 3;

    private readonly IStockItemRepository _stockItemRepository;
    private readonly IProductCatalogReader _productCatalogReader;
    private readonly ILogger<InventoryApplicationService> _logger;

    internal InventoryApplicationService(IStockItemRepository stockItemRepository, IProductCatalogReader productCatalogReader, ILogger<InventoryApplicationService> logger)
    {
        _stockItemRepository = stockItemRepository;
        _productCatalogReader = productCatalogReader;
        _logger = logger;
    }

    public async Task<Result> SetInitialStockAsync(Guid productId, Guid requestingVendorId, int quantity, CancellationToken cancellationToken)
    {
        var product = await _productCatalogReader.GetSnapshotAsync(productId, cancellationToken);
        if (product is null || product.VendorId != requestingVendorId)
        {
            return Result.Failure("Product not found", ErrorType.NotFound);
        }
        var existing = await _stockItemRepository.GetByProductIdAsync(productId, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure("Stock record already exists for this product", ErrorType.Conflict);
        }
        if (quantity < 0)
        {
            return Result.Failure("Quantity cannot be negative.", ErrorType.Validation);
        }
        var stockItem = Domain.StockItem.Create(productId, quantity);
        await _stockItemRepository.AddAsync(stockItem, cancellationToken);
        await _stockItemRepository.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Stock initialized for product {ProductId}: {Quantity} units", productId, quantity);
        return Result.Success();
    }

    public async Task<Result> ReserveAsync(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            var stockItem = await _stockItemRepository.GetByProductIdAsync(productId, cancellationToken);
            if (stockItem is null)
                return Result.Failure("No stock record for this product.", ErrorType.NotFound);
            
            var reserveResult = stockItem.Reserve(quantity);
            if (!reserveResult.IsSuccess)
                return reserveResult;

            try
            {
                await _stockItemRepository.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning(
                    "Concurrency conflict reserving {Quantity} of product {ProductId}, attempt {Attempt}/{MaxRetries}",
                    quantity, productId, attempt, MaxRetries);
            }
        }
        _logger.LogError("Failed to reserve stock for product {ProductId} after {MaxRetries} attempts due to contention", productId, MaxRetries);
        return Result.Failure("Could not reserve stock due to high demand. Please try again.", ErrorType.Conflict);
    }

    public async Task<Result> ReleaseAsync(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            var stockItem = await _stockItemRepository.GetByProductIdAsync(productId, cancellationToken);
            if (stockItem is null)
            {
                return Result.Failure("No stock record for this product.", ErrorType.NotFound);
            }

            stockItem.Release(quantity);

            try
            {
                await _stockItemRepository.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning("Concurrency conflict releasing stock for product {ProductId}, attempt {Attempt}/{MaxRetries}", productId, attempt, MaxRetries);
            }
        }

        return Result.Failure("Could not release stock due to contention. Please try again.", ErrorType.Conflict);
    }
}