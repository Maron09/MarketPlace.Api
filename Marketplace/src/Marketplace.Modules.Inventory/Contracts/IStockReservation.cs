namespace Marketplace.Modules.Inventory.Contracts;

public interface IStockReservation
{
    Task<bool> TryReserveAsync(Guid productId, int quantity, CancellationToken cancellationToken);
}

public interface IStockRelease
{
    Task ReleaseAsync(Guid productId, int quantity, CancellationToken cancellationToken);
}

public interface IStockAvailabilityReader
{
    Task<int> GetAvailableQuantityAsync(Guid productId, CancellationToken cancellationToken);
}