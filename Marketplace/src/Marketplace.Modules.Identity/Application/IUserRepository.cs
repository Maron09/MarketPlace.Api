using Marketplace.Modules.Identity.Domain;

namespace Marketplace.Modules.Identity.Application
{
    internal interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    }
}