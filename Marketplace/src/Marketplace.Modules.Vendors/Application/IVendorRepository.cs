using Marketplace.Modules.Vendors.Domain;

namespace Marketplace.Modules.Vendors.Application
{
    internal interface IVendorRepository
    {
        Task<bool> HasVendorProfileAsync(Guid userId, CancellationToken cancellationToken);
        Task<Vendor?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<Vendor?> GetByIdAsync(Guid vendorId, CancellationToken cancellationToken);
        Task AddAsync(Vendor vendor, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}