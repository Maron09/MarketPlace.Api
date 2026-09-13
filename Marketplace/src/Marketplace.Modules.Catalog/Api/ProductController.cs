using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Marketplace.Modules.Catalog.Application;
using Marketplace.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Marketplace.Modules.Catalog.Api
{
    public sealed record UpdateProductRequestDto(string Name, string? Description, decimal Price);

    [ApiController]
    [Route("api/products")]
    public sealed class ProductController : ControllerBase
    {
        private readonly CatalogApplicationService _catalogApplicationService;

        public ProductController(CatalogApplicationService catalogApplicationService)
        {
            _catalogApplicationService = catalogApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> Browse([FromQuery] int page =1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var products = await _catalogApplicationService.BrowseAsync(page, pageSize, cancellationToken);
            return Ok(products);
        }

        [HttpGet("{productId:guid}")]
        public async Task<IActionResult> GetById(Guid productId, CancellationToken cancellationToken)
        {
            var result = await _catalogApplicationService.GetByIdAsync(productId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result);
        }

        [Authorize(Roles ="Vendor")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDto request, CancellationToken cancellationToken)
        {
            var productId = await _catalogApplicationService.CreateAsync(
                new CreateProductRequest(GetAuthenticatedUserId(), request.Name, request.Description, request.Price), cancellationToken);
            
            return StatusCode(StatusCodes.Status201Created, new { productId });
        }

        [Authorize(Roles = "Vendor")]
        [HttpPut("{productId:guid}")]
        public async Task<IActionResult> Update(Guid productId, [FromBody] UpdateProductRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _catalogApplicationService.UpdateAsync(productId,
            GetAuthenticatedUserId(), request.Name, request.Description, request.Price, cancellationToken);

            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }

        [Authorize(Roles = "Vendor")]
        [HttpPost("{productId:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid productId,CancellationToken cancellationToken)
        {
            var result = await _catalogApplicationService.DeactivateAsync(productId, GetAuthenticatedUserId(), cancellationToken);
            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }

        [Authorize(Roles = "Vendor")]
        [HttpPost("{productId:guid}/reactivate")]
        public async Task<IActionResult> Reactivate(Guid productId, CancellationToken cancellationToken)
        {
            var result = await _catalogApplicationService.ReactivateAsync(productId, GetAuthenticatedUserId(), cancellationToken);
            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }

        [Authorize(Roles = "Vendor")]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var products = await _catalogApplicationService.GetByVendorIdAsync(GetAuthenticatedUserId(), page, pageSize, cancellationToken);
            return Ok(products);
        }

        private Guid GetAuthenticatedUserId()
            => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? throw new InvalidOperationException("Authenticated request missing sub claim."));

    }
}