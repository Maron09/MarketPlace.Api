using Marketplace.Infrastructure.Shared.Persistence;
using Marketplace.Modules.Vendors.Application;
using Marketplace.Modules.Vendors.Domain;
using Microsoft.EntityFrameworkCore;


namespace Marketplace.Modules.Vendors.Infrastructure
{
    internal sealed class VendorRepository : IVendorRepository
    {
        private readonly MarketplaceDbContext _dbContext;

        public VendorRepository(MarketplaceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> HasVendorProfileAsync(Guid userId, CancellationToken cancellationToken)
            => await _dbContext.Set<Vendor>().AnyAsync(v => v.UserId == userId, cancellationToken);
        
        public async Task<Vendor?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => await _dbContext.Set<Vendor>().FirstOrDefaultAsync(v => v.UserId == userId, cancellationToken);
        
        public async Task<Vendor?> GetByIdAsync(Guid vendorId, CancellationToken cancellationToken)
            => await _dbContext.Set<Vendor>().FirstOrDefaultAsync(v => v.Id == vendorId, cancellationToken);
        
        public async Task AddAsync(Vendor vendor, CancellationToken cancellationToken)
        {
            await _dbContext.Set<Vendor>().AddAsync(vendor, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _dbContext.SaveChangesAsync(cancellationToken);
    }
}