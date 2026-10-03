using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Orders.Application;
using Marketplace.Modules.Orders.Domain;
using Microsoft.EntityFrameworkCore;


namespace Marketplace.Modules.Orders.Infrastructure;

internal sealed class IdempotencyKeyRepository : IIdempotencyKeyRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public IdempotencyKeyRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IdempotencyKey?> FindAsync(Guid key, CancellationToken cancellationToken)
        => _dbContext.Set<IdempotencyKey>().FirstOrDefaultAsync(x => x.Key == key, cancellationToken);
    
    public async Task AddAsync(IdempotencyKey record, CancellationToken cancellationToken)
        => await _dbContext.Set<IdempotencyKey>().AddAsync(record, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}