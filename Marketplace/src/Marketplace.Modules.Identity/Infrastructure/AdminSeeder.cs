using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Identity.Application;
using Marketplace.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Modules.Identity.Infrastructure
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(MarketplaceDbContext dbContext, IPasswordHasher passwordHasher, string email, string password)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var exists = await dbContext.Set<User>().AnyAsync(u => u.Email == normalizedEmail);
            if (exists)
            {
                return;
            }
            var admin = User.Register(normalizedEmail, passwordHasher.Hash(password), UserRole.Admin);
            await dbContext.Set<User>().AddAsync(admin);
            await dbContext.SaveChangesAsync();
        }
    }
}