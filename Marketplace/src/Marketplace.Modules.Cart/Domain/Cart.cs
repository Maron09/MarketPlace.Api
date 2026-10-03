
namespace Marketplace.Modules.Cart.Domain;


internal sealed class Cart
{
    private readonly List<CartItem> _items = new();

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    private Cart() { }

    private Cart(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public static Cart Create(Guid userId) => new(Guid.NewGuid(), userId);

    public CartItem AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            existing.IncreaseQuantity(quantity);
            UpdatedAtUtc = DateTime.UtcNow;
            return existing;
        }

        var item = CartItem.Create(Id, productId, productName, unitPrice, quantity);
        _items.Add(item);
        UpdatedAtUtc = DateTime.UtcNow;
        return item;
    }

    public bool UpdateItemQuantity(Guid cartItemId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.Id == cartItemId);
        if (item is null)
        {
            return false;
        }
        if (quantity <= 0)
        {
            _items.Remove(item);
        }
        else
        {
            item.SetQuantity(quantity);
        }

        UpdatedAtUtc = DateTime.UtcNow;
        return true;
    }

    public bool RemoveItem(Guid cartItemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == cartItemId);
        if (item is null)
        {
            return false;
        }

        _items.Remove(item);
        UpdatedAtUtc = DateTime.UtcNow;
        return true;
    }

    public void ClearItems()
    {
        _items.Clear();
        UpdatedAtUtc = DateTime.UtcNow;
    }
}