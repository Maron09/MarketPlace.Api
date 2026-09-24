using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Cart.Application;
using Microsoft.EntityFrameworkCore;


namespace Marketplace.Modules.Cart.Infrastructure;

internal sealed class CartRepository : ICartRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public CartRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Domain.Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => await _dbContext.Set<Domain.Cart>()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    
    public async Task AddAsync(Domain.Cart cart, CancellationToken cancellationToken)
        => await _dbContext.Set<Domain.Cart>().AddAsync(cart, cancellationToken);
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}