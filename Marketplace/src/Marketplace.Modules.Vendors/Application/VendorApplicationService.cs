using Marketplace.SharedKernel;
using Microsoft.Extensions.Logging;

namespace Marketplace.Modules.Vendors.Application
{
    public sealed record ApplyAsVendorRequest(Guid UserId, string BusinessName, string? Description);

    public sealed class VendorApplicationService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ILogger<VendorApplicationService> _logger;

        internal VendorApplicationService(IVendorRepository vendorRepository, ILogger<VendorApplicationService> logger)
        {
            _vendorRepository = vendorRepository;
            _logger = logger;
        }

        public async Task<Result<Guid>> ApplyAsync(ApplyAsVendorRequest request, CancellationToken cancellationToken)
        {
            if (await _vendorRepository.HasVendorProfileAsync(request.UserId, cancellationToken))
            {
                _logger.LogWarning("Rejected vendor application attempt for user {UserId}", request.UserId);
                return Result<Guid>.Failure("A vendor profile already exists for this user.", ErrorType.Conflict);
            }
            
            var vendor = Domain.Vendor.Apply(request.UserId, request.BusinessName, request.Description);
            await _vendorRepository.AddAsync(vendor, cancellationToken);
            
            _logger.LogInformation("Vendor application successful for user {UserId}", request.UserId);
            return Result<Guid>.Success(vendor.Id);
        }

        public async Task<Result> ApproveAsync(Guid vendorId, Guid reviewerId, CancellationToken cancellationToken)
            => await ApplyTransitionAsync(vendorId, v => v.Approve(reviewerId), cancellationToken);

        public async Task<Result> RejectAsync(Guid vendorId, Guid reviewerId, CancellationToken cancellationToken)
            => await ApplyTransitionAsync(vendorId, v => v.Reject(reviewerId), cancellationToken);

        public async Task<Result> SuspendAsync(Guid vendorId, Guid reviewerId, CancellationToken cancellationToken)
            => await ApplyTransitionAsync(vendorId, v => v.Suspend(reviewerId), cancellationToken);

        public async Task<Result> ReApplyAsync(Guid vendorId, CancellationToken cancellationToken)
            => await ApplyTransitionAsync(vendorId, v => v.ReApply(), cancellationToken);

        private async Task<Result> ApplyTransitionAsync(
            Guid vendorId,
            Func<Domain.Vendor, Result> transition,
            CancellationToken cancellationToken)
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId, cancellationToken);
            if (vendor is null)
            {
                _logger.LogWarning("Vendor info not found");
                return Result.Failure("Vendor not found.", ErrorType.NotFound);
            }

            var result = transition(vendor);
            if (!result.IsSuccess)
            {
                return result;
            }

            await _vendorRepository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }   
    }
}