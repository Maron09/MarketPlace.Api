using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Identity.Application;
using Marketplace.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;


namespace Marketplace.Modules.Identity.Infrastructure
{
    internal sealed class UserRepository : IUserRepository
    {
        private readonly MarketplaceDbContext _dbContext;

        public UserRepository(MarketplaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<User>()
                .AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _dbContext.Set<User>().AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<User>()
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}