using Marketplace.Modules.Vendors.Application;
using Marketplace.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Marketplace.Modules.Vendors.Api
{
    public sealed record ApplyAsVendorRequestDto(Guid UserId, string BusinessName, string? Description);
    public sealed record ReviewVendorRequestDto(Guid ReviewerId);

    [ApiController]
    [Route("api/vendors")]
    public sealed class VendorController : ControllerBase
    {
        private readonly VendorApplicationService _vendorApplicationService;

        public VendorController(VendorApplicationService vendorApplicationService)
        {
            _vendorApplicationService = vendorApplicationService;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyAsVendorRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _vendorApplicationService.ApplyAsync(
                new ApplyAsVendorRequest(request.UserId, request.BusinessName, request.Description), cancellationToken);
            
            if (!result.IsSuccess)
                return this.ToActionResult(result);
            
            return StatusCode(StatusCodes.Status201Created, new { vendorId = result.Value });
        }

        [HttpPost("{vendorId:guid}/approve")]
        public async Task<IActionResult> Approve(Guid vendorId, [FromBody] ReviewVendorRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _vendorApplicationService.ApproveAsync(vendorId, request.ReviewerId, cancellationToken);
            // if (!result.IsSuccess)
            //     // "Vendor not found" and "wrong status for this transition" are both
            //     // just IsSuccess == false right now — no way to tell them apart here.
            //     return Conflict(new { error = result.Error });
            
            // return NoContent();
            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }

        [HttpPost("{vendorId:guid}/reject")]
        public async Task<IActionResult> Reject(Guid vendorId, [FromBody] ReviewVendorRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _vendorApplicationService.RejectAsync(vendorId, request.ReviewerId, cancellationToken);
            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }

        [HttpPost("{vendorId:guid}/suspend")]
        public async Task<IActionResult> Suspend(Guid vendorId, [FromBody] ReviewVendorRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _vendorApplicationService.SuspendAsync(vendorId, request.ReviewerId, cancellationToken);
            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }

        [HttpPost("{vendorId:guid}/reapply")]
        public async Task<IActionResult> ReApply(Guid vendorId, CancellationToken cancellationToken)
        {
            var result = await _vendorApplicationService.ReApplyAsync(vendorId,  cancellationToken);
            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }
    }
}