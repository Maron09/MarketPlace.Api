namespace Marketplace.Modules.Catalog.Contracts;

public interface IProductCatalogReader
{
    Task<ProductSnapShotDto?> GetSnapshotAsync(Guid productId, CancellationToken cancellationToken);
}

public sealed record ProductSnapShotDto(Guid ProductId, Guid VendorId, string Name, decimal Price, bool IsActive);