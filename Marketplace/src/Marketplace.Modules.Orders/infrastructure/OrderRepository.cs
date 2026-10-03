using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Orders.Application;
using Marketplace.Modules.Orders.Domain;
using Microsoft.EntityFrameworkCore;


namespace Marketplace.Modules.Orders.Infrastructure;


internal sealed class OrderRepository : IOrderRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public OrderRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
        => await _dbContext.Set<Order>()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    
    public async Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken)
        => await _dbContext.Set<Order>()
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    
    public async Task AddAsync(Order order, CancellationToken cancellationToken)
        => await _dbContext.Set<Order>().AddAsync(order, cancellationToken);
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}