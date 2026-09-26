

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
    private readonly ICurrentUser _currentUser;

    public CartController(CartApplicationService cartApplicationService, ICurrentUser currentUser)
    {
        _cartApplicationService = cartApplicationService;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var cart = await _cartApplicationService.GetCartAsync(_currentUser.UserId, cancellationToken);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddItemRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _cartApplicationService.AddItemAsync(
            _currentUser.UserId, request.ProductId, request.Quantity, cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, new { cartItemId = result.Value })
            : this.ToActionResult(result);
    }

    [HttpPut("items/{cartItemId:guid}")]
    public async Task<IActionResult> UpdateQuantity(Guid cartItemId, [FromBody] UpdateQuantityRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _cartApplicationService.UpdateItemQuantityAsync(
            _currentUser.UserId, cartItemId, request.Quantity, cancellationToken);

        return result.IsSuccess ? NoContent() : this.ToActionResult(result);
    }

    [HttpDelete("items/{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid cartItemId, CancellationToken cancellationToken)
    {
        var result = await _cartApplicationService.RemoveItemAsync(_currentUser.UserId, cartItemId, cancellationToken);
        return result.IsSuccess ? NoContent() : this.ToActionResult(result);
    }
}