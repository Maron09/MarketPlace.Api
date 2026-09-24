

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Marketplace.Modules.Cart.Application;
using Marketplace.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Marketplace.Modules.Cart.Api;

public sealed record AddItemRequestDto(Guid ProductId, int Quantity);
public sealed record UpdateQuantityRequestDto(int Quantity);

[ApiController]
[Route("api/cart")]
[Authorize]
public sealed class CartController : ControllerBase
{
    private readonly CartApplicationService _cartApplicationService;

    public CartController(CartApplicationService cartApplicationService)
    {
        _cartApplicationService = cartApplicationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var cart = await _cartApplicationService.GetCartAsync(GetAuthenticatedUserId(), cancellationToken);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddItemRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _cartApplicationService.AddItemAsync(
            GetAuthenticatedUserId(), request.ProductId, request.Quantity, cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, new { cartItemId = result.Value })
            : this.ToActionResult(result);
    }

    [HttpPut("items/{cartItemId:guid}")]
    public async Task<IActionResult> UpdateQuantity(Guid cartItemId, [FromBody] UpdateQuantityRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _cartApplicationService.UpdateItemQuantityAsync(
            GetAuthenticatedUserId(), cartItemId, request.Quantity, cancellationToken);

        return result.IsSuccess ? NoContent() : this.ToActionResult(result);
    }

    [HttpDelete("items/{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid cartItemId, CancellationToken cancellationToken)
    {
        var result = await _cartApplicationService.RemoveItemAsync(GetAuthenticatedUserId(), cartItemId, cancellationToken);
        return result.IsSuccess ? NoContent() : this.ToActionResult(result);
    }

    private Guid GetAuthenticatedUserId()
        => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new InvalidOperationException("Authenticated request missing sub claim."));
}