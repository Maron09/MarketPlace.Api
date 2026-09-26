using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Marketplace.Modules.Inventory.Application;
using Marketplace.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Modules.Inventory.Api;

public sealed record SetInitialStockRequestDto(int Quantity);


[ApiController]
[Route("api/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly InventoryApplicationService _inventoryApplicationService;
    private readonly ICurrentUser _currentUser;

    public InventoryController(InventoryApplicationService inventoryApplicationService, ICurrentUser currentUser)
    {
        _inventoryApplicationService = inventoryApplicationService;
        _currentUser = currentUser;
    }

    [Authorize(Roles = "Vendor")]
    [HttpPost("products/{productId:guid}/stock")]
    public async Task<IActionResult> SetInitialStock(Guid productId, [FromBody] SetInitialStockRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _inventoryApplicationService.SetInitialStockAsync(productId, _currentUser.UserId, request.Quantity, cancellationToken);
        return result.IsSuccess ? NoContent() : this.ToActionResult(result);
    }
}