using Marketplace.Modules.Orders.Domain;


namespace Marketplace.Modules.Orders.Application;

internal interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}