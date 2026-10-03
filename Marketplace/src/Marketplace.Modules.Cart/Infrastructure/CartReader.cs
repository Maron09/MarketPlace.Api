using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Cart.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Marketplace.Modules.Cart.Infrastructure;

internal sealed class CartReader : ICartReader
{
    private readonly MarketplaceDbContext _dbContext;

    public CartReader(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CartLineDto>> GetCartLinesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cart = await _dbContext.Set<Domain.Cart>()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (cart is null)
        {
            return Array.Empty<CartLineDto>();
        }

        return cart.Items.Select(i => new CartLineDto(i.ProductId, i.Quantity, i.UnitPriceSnapshot)).ToList();
    }

    public async Task ClearCartAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cart = await _dbContext.Set<Domain.Cart>()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        
        cart?.ClearItems();
    }
}