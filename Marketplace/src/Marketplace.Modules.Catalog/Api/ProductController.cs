using Marketplace.Modules.Catalog.Application;
using Marketplace.SharedKernel;
using Microsoft.AspNetCore.Mvc;


namespace Marketplace.Modules.Catalog.Api
{
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
    }
}