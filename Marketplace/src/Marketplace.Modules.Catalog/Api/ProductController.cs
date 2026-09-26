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
        private readonly CategoryApplicationService _categoryApplicationService;
        private readonly ICurrentUser _currentUser;

        public ProductController(CatalogApplicationService catalogApplicationService, CategoryApplicationService categoryApplicationService, ICurrentUser currentUser)
        {
            _catalogApplicationService = catalogApplicationService;
            _categoryApplicationService = categoryApplicationService;
            _currentUser = currentUser;
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
                new CreateProductRequest(_currentUser.UserId, request.Name, request.Description, request.Price), cancellationToken);
            
            return StatusCode(StatusCodes.Status201Created, new { productId });
        }

        [Authorize(Roles = "Vendor")]
        [HttpPut("{productId:guid}")]
        public async Task<IActionResult> Update(Guid productId, [FromBody] UpdateProductRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _catalogApplicationService.UpdateAsync(productId,
            _currentUser.UserId, request.Name, request.Description, request.Price, cancellationToken);

            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }

        [Authorize(Roles = "Vendor")]
        [HttpPost("{productId:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid productId,CancellationToken cancellationToken)
        {
            var result = await _catalogApplicationService.DeactivateAsync(productId, _currentUser.UserId, cancellationToken);
            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }

        [Authorize(Roles = "Vendor")]
        [HttpPost("{productId:guid}/reactivate")]
        public async Task<IActionResult> Reactivate(Guid productId, CancellationToken cancellationToken)
        {
            var result = await _catalogApplicationService.ReactivateAsync(productId, _currentUser.UserId, cancellationToken);
            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }

        [Authorize(Roles = "Vendor")]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var products = await _catalogApplicationService.GetByVendorIdAsync(_currentUser.UserId, page, pageSize, cancellationToken);
            return Ok(products);
        }
        [Authorize(Roles = "Vendor")]
        [HttpPost("{productId:guid}/categories/{categoryId:guid}")]
        public async Task<IActionResult> AssignCategory(Guid productId, Guid categoryId, CancellationToken cancellationToken)
        {
            var result = await _categoryApplicationService.AssignToProductAsync(productId, categoryId, _currentUser.UserId, cancellationToken);
            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }

        [Authorize(Roles = "Vendor")]
        [HttpDelete("{productId:guid}/categories/{categoryId:guid}")]
        public async Task<IActionResult> RemoveCategory(Guid productId, Guid categoryId, CancellationToken cancellationToken)
        {
            var result = await _categoryApplicationService.RemoveFromProductAsync(productId, categoryId, _currentUser.UserId, cancellationToken);
            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }

        [Authorize(Roles = "Vendor")]
        [HttpPost("{productId:guid}/images")]
        public async Task<IActionResult> UploadImage(Guid productId, IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest(new { error = "No file was provided." });
            }

            await using var stream = file.OpenReadStream();

            var result = await _catalogApplicationService.AddImageAsync(
                productId,
                _currentUser.UserId,
                stream,
                file.FileName,
                file.Length,
                cancellationToken);
            Console.WriteLine($"Received file: {file.FileName}, ContentType: '{file.ContentType}', Length: {file.Length}");
            return result.IsSuccess
                ? StatusCode(StatusCodes.Status201Created, new { imageId = result.Value })
                : this.ToActionResult(result);
        }
    }
}