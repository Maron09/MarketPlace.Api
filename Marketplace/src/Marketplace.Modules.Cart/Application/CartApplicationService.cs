using Marketplace.Modules.Catalog.Contracts;
using Marketplace.Modules.Inventory.Contracts;
using Marketplace.SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Marketplace.Modules.Cart.Application;

public sealed class CartApplicationService
{

    public sealed record CartItemDto(Guid Id, Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);
    public sealed record CartDto(Guid Id, Guid UserId, IReadOnlyList<CartItemDto> Items, decimal Total);
    private readonly ICartRepository _cartRepository;
    private readonly IProductCatalogReader _productCatalogReader;
    private readonly IStockAvailabilityReader _stockAvailabilityReader;
    private readonly ILogger<CartApplicationService> _logger;

    internal CartApplicationService(ICartRepository cartRepository, IProductCatalogReader productCatalogReader, IStockAvailabilityReader stockAvailabilityReader, ILogger<CartApplicationService> logger)
    {
        _cartRepository = cartRepository;
        _productCatalogReader = productCatalogReader;
        _stockAvailabilityReader = stockAvailabilityReader;
        _logger = logger;
    }

    private async Task<Domain.Cart> GetOrCreateCartAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart is not null)
        {
            return cart;
        }
        var newCart = Domain.Cart.Create(userId);
        await _cartRepository.AddAsync(newCart, cancellationToken);
        await _cartRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created new cart {CartId} for user {UserId}", newCart.Id, userId);
        return newCart;
    }

    public async Task<CartDto> GetCartAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        return ToDto(cart);
    }

    public async Task<Result<Guid>> AddItemAsync(
    Guid userId, Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var product = await _productCatalogReader.GetSnapshotAsync(productId, cancellationToken);
        if (product is null)
        {
            return Result<Guid>.Failure("Product not found.", ErrorType.NotFound);
        }

        if (!product.IsActive)
        {
            return Result<Guid>.Failure("Product is not currently available.", ErrorType.Validation);
        }

        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        
        var isNewCart = cart is null;
        if (cart is null)
        {
            cart = Domain.Cart.Create(userId);
            await _cartRepository.AddAsync(cart, cancellationToken);
        }

        var existingItemQuantity = cart.Items.FirstOrDefault(i => i.ProductId == productId)?.Quantity ?? 0;
        var totalDesiredQuantity = existingItemQuantity + quantity;

        var availableQuantity = await _stockAvailabilityReader.GetAvailableQuantityAsync(productId, cancellationToken);
        if (totalDesiredQuantity > availableQuantity)
        {
            return Result<Guid>.Failure(
                $"Only {availableQuantity} unit(s) of {product.Name} available.",
                ErrorType.Conflict);
        }

        var item = cart.AddItem(product.ProductId, product.Name, product.Price, quantity);

        await _cartRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Added product {ProductId} (qty {Quantity}) to cart {CartId}{NewCartNote}",
            productId, quantity, cart.Id, isNewCart ? " (new cart created)" : "");

        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result> UpdateItemQuantityAsync(Guid userId, Guid cartItemId, int quantity, CancellationToken cancellationToken)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);

        var updated = cart.UpdateItemQuantity(cartItemId, quantity);
        if (!updated)
        {
            return Result.Failure("Cart item not found.", ErrorType.NotFound);
        }

        await _cartRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Updated quantity for cart item {CartItemId} to {Quantity}", cartItemId, quantity);
        return Result.Success();
    }

    public async Task<Result> RemoveItemAsync(Guid userId, Guid cartItemId, CancellationToken cancellationToken)
    {
        var cart = await GetOrCreateCartAsync(userId, cancellationToken);

        var removed = cart.RemoveItem(cartItemId);
        if (!removed)
        {
            return Result.Failure("Cart item not found.", ErrorType.NotFound);
        }

        await _cartRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Removed cart item {CartItemId}", cartItemId);
        return Result.Success();
    }

    private static CartDto ToDto(Domain.Cart cart)
    {
        var items = cart.Items
            .Select(i => new CartItemDto(i.Id, i.ProductId, i.ProductNameSnapshot, i.UnitPriceSnapshot, i.Quantity))
            .ToList();

        var total = items.Sum(i => i.UnitPrice * i.Quantity);
        return new CartDto(cart.Id, cart.UserId, items, total);
    }
}