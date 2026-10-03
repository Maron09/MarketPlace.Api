namespace Marketplace.Modules.Orders.Domain;

internal sealed class OrderItem
{
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public Guid VendorId { get; private set; }

    public Guid ProductId { get; private set; }

    public string ProductNameSnapshot { get; private set; }
        = null!;

    public decimal UnitPriceSnapshot { get; private set; }

    public int Quantity { get; private set; }

    public decimal TotalPrice =>
        UnitPriceSnapshot * Quantity;

    private OrderItem() { }

    private OrderItem(
        Guid id,
        Guid orderId,
        Guid productId,
        Guid vendorId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        Id = id;

        OrderId = orderId;

        ProductId = productId;

        VendorId = vendorId;

        ProductNameSnapshot = productName;

        UnitPriceSnapshot = unitPrice;

        Quantity = quantity;
    }

    public static OrderItem Create(
        Guid orderId,
        Guid productId,
        Guid vendorId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        return new(
            Guid.NewGuid(),
            orderId,
            productId,
            vendorId,
            productName,
            unitPrice,
            quantity);
    }
}