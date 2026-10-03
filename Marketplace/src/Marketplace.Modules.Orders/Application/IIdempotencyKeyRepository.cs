using Marketplace.Modules.Orders.Domain;


namespace Marketplace.Modules.Orders.Application;

public interface IIdempotencyKeyRepository
{
    Task<IdempotencyKey?> FindAsync(Guid key, CancellationToken cancellationToken);
    Task AddAsync(IdempotencyKey record, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}