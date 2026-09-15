using Marketplace.Modules.Catalog.Application;
using Marketplace.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Marketplace.Modules.Catalog.Api
{
    public sealed record CreateCateoryRequestDto(string Name);
    public sealed record RenameCategoryRequestDto(string Name);

    [ApiController]
    [Route("api/categories")]
    public sealed class CategoryController : ControllerBase
    {
        private readonly CategoryApplicationService _categoryApplicationService;

        public CategoryController(CategoryApplicationService categoryApplicationService)
        {
            _categoryApplicationService = categoryApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var categories = await _categoryApplicationService.GetAllAsync(cancellationToken);
            return Ok(categories);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCateoryRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _categoryApplicationService.CreateAsync(request.Name, cancellationToken);
            if (!result.IsSuccess)
                return this.ToActionResult(result);
            
            return StatusCode(StatusCodes.Status201Created, new { categoryId = result.Value });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{categoryId:guid}")]
        public async Task<IActionResult> Rename(Guid categoryId, [FromBody] RenameCategoryRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _categoryApplicationService.RenameAsync(categoryId, request.Name, cancellationToken);
            return result.IsSuccess ? NoContent() : this.ToActionResult(result);
        }
    }
}