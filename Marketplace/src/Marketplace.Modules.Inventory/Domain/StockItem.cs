using Marketplace.SharedKernel;

namespace Marketplace.Modules.Inventory.Domain;

internal sealed class StockItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int QuantityOnHand { get; private set; }
    public uint RowVersion { get; private set; }

    private StockItem() { }

    private StockItem(Guid id, Guid productId, int quantityOnHand)
    {
        Id = id;
        ProductId = productId;
        QuantityOnHand = quantityOnHand;
    }
    public static StockItem Create(Guid productId, int initialQuantity)
        => new(Guid.NewGuid(), productId, initialQuantity);
    
    public Result Reserve(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure("Reservation quantity must be positive.", ErrorType.Validation);
        }
        if (QuantityOnHand < quantity)
        {
            return Result.Failure("Insufficient stock.", ErrorType.Conflict);
        }
        QuantityOnHand -= quantity;
        return Result.Success();
    }

    public Result Release(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure("Release quantity must be positive.", ErrorType.Validation);
        }

        QuantityOnHand += quantity;
        return Result.Success();
    }
}