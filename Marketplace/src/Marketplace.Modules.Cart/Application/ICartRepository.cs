
namespace Marketplace.Modules.Cart.Application;

internal interface ICartRepository
{
    Task<Domain.Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task AddAsync(Domain.Cart cart, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}