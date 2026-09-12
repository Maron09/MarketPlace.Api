using Marketplace.SharedKernel;

namespace Marketplace.Modules.Vendors.Domain
{
    internal sealed class Vendor
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string BusinessName { get; private set; } = null!;
        public string? Description { get; private set; }
        public VendorStatus Status { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? ReviewedAtUtc { get; private set; }
        public Guid? ReviewedBy { get; private set; }

        private Vendor() { }

        private Vendor(Guid id, Guid userId, string businessName, string? description)
        {
            Id = id;
            UserId = userId;
            BusinessName = businessName;
            Description = description;
            Status = VendorStatus.Pending;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public static Vendor Apply(Guid userId, string businessName, string? description)
            => new(Guid.NewGuid(), userId, businessName, description);
        

        public Result Approve(Guid reviewerId)
        {
            if (Status is not (VendorStatus.Pending or VendorStatus.Suspended))
                return Result.Failure($"Cannot approve a vendor in status '{Status}'.", ErrorType.Conflict);
            
            Status = VendorStatus.Approved;
            ReviewedAtUtc = DateTime.UtcNow;
            ReviewedBy = reviewerId;
            return Result.Success();
        }


        public Result Reject(Guid reviewerId)
        {
            if (Status != VendorStatus.Pending)
                return Result.Failure($"Cannot reject a vendor in status '{Status}'.", ErrorType.Conflict);

            Status = VendorStatus.Rejected;
            ReviewedAtUtc = DateTime.UtcNow;
            ReviewedBy = reviewerId;
            return Result.Success();
        }

        public Result Suspend(Guid reviewerId)
        {
            if (Status != VendorStatus.Approved)
                return Result.Failure($"Cannot suspend a vendor in status '{Status}'.", ErrorType.Conflict);

            Status = VendorStatus.Suspended;
            ReviewedAtUtc = DateTime.UtcNow;
            ReviewedBy = reviewerId;
            return Result.Success();
        }

        public Result ReApply()
        {
            if (Status != VendorStatus.Rejected)
                return Result.Failure($"Cannot re-apply from status '{Status}'.", ErrorType.Conflict);

            Status = VendorStatus.Pending;
            ReviewedAtUtc = null;
            ReviewedBy = null;
            return Result.Success();
        }
    }
}