namespace Marketplace.Modules.Cart.Contracts;

public interface ICartReader
{
    Task<IReadOnlyList<CartLineDto>> GetCartLinesAsync(Guid userId, CancellationToken cancellationToken);
    Task ClearCartAsync(Guid userId, CancellationToken cancellationToken);

}


public sealed record CartLineDto(Guid ProductId, int Quantity, decimal SnapshottedUnitPrice);