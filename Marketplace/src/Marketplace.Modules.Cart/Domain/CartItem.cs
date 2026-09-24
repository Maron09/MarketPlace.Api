
namespace Marketplace.Modules.Cart.Domain;


internal sealed class CartItem
{
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductNameSnapshot { get; private set; } = null!;
    public decimal UnitPriceSnapshot { get; private set; }
    public int Quantity { get; private set; }

    private CartItem() { }

    private CartItem(
        Guid id,
        Guid cartId,
        Guid productId,
        string productName,
        decimal unitPrice,
        int quantity)
    {
        Id = id;
        CartId = cartId;
        ProductId = productId;
        ProductNameSnapshot = productName;
        UnitPriceSnapshot = unitPrice;
        Quantity = quantity;
    }

    public static CartItem Create(Guid cartId, Guid productId, string productName, decimal unitPrice, int quantity)
        => new(Guid.NewGuid(), cartId, productId, productName, unitPrice, quantity);
    
    public void IncreaseQuantity(int amount) => Quantity += amount;

    public void SetQuantity(int quantity) => Quantity = quantity;
}