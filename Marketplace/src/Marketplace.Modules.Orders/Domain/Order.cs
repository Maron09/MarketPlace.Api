using Marketplace.SharedKernel;

namespace Marketplace.Modules.Orders.Domain;

internal sealed class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal TotalAmount { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyList<OrderItem> Items
        => _items.AsReadOnly();

    private Order() { }

    private Order(
        Guid id,
        Guid userId)
    {
        Id = id;
        UserId = userId;
        Status = OrderStatus.Pending;

        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Order Create(Guid userId) => new(Guid.NewGuid(), userId);

    public void AddItem(
        Guid productId,
        Guid vendorId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        var item = OrderItem.Create(
            Id,
            productId,
            vendorId,
            productName,
            unitPrice,
            quantity);

        _items.Add(item);

        RecalculateTotal();
    }

    public Result MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Failure($"Cannot mark order as paid from status '{Status}'.", ErrorType.Conflict);
        }
        if (_items.Count == 0)
        {
            return Result.Failure("Cannot mark an empty order as paid.", ErrorType.Validation);
        }
        Status = OrderStatus.Paid;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is not (OrderStatus.Pending or OrderStatus.Paid))
        {
            return Result.Failure($"Cannot cancel an order in status '{Status}'.", ErrorType.Conflict);
        }
        Status = OrderStatus.Cancelled;
        return Result.Success();
    }

    public Result MarkAsCompleted()
    {
        if (Status != OrderStatus.Completed)
        {
            return Result.Failure($"Cannot complete an order in status '{Status}'.", ErrorType.Conflict);
        }

        Status = OrderStatus.Completed;
        return Result.Success();
    }

    private void RecalculateTotal()
    {
        TotalAmount =
            _items.Sum(i => i.TotalPrice);
    }
}