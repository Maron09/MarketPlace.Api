using Marketplace.SharedKernel;

namespace Marketplace.Modules.Vendors.Application
{
    public sealed record ApplyAsVendorRequest(Guid UserId, string BusinessName, string? Description);

    public sealed class VendorApplicationService
    {
        private readonly IVendorRepository _vendorRepository;

        internal VendorApplicationService(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        public async Task<Result<Guid>> ApplyAsync(ApplyAsVendorRequest request, CancellationToken cancellationToken)
        {
            if (await _vendorRepository.HasVendorProfileAsync(request.UserId, cancellationToken))
                return Result<Guid>.Failure("A vendor profile already exists for this user.", ErrorType.Conflict);
            
            var vendor = Domain.Vendor.Apply(request.UserId, request.BusinessName, request.Description);
            await _vendorRepository.AddAsync(vendor, cancellationToken);

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