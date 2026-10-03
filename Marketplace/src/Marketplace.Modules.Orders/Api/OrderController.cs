using Marketplace.Modules.Orders.Application;
using Marketplace.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Marketplace.Modules.Orders.Api;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class OrderController : ControllerBase
{
    private readonly OrderApplicationService _orderApplicationService;
    private readonly ICurrentUser _currentUser;

    public OrderController(OrderApplicationService orderApplicationService, ICurrentUser currentUser)
    {
        _orderApplicationService = orderApplicationService;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromHeader(Name = "Idempotency-Key")] string? idempotencyKeyHeader, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(idempotencyKeyHeader, out var idempotencyKey))
        {
            return BadRequest(new { error = "A valid Idempotency-Key header is required." });
        }
        var result = await _orderApplicationService.PlaceOrderAsync(_currentUser.UserId, idempotencyKey, cancellationToken);
        if (!result.IsSuccess)
        {
            return this.ToActionResult(result);
        }
        else
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, new { orderId = result.Value });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _orderApplicationService.GetByIdAsync(id, _currentUser.UserId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : this.ToActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var orders = await _orderApplicationService.GetMyOrdersAsync(_currentUser.UserId, page, pageSize, cancellationToken);
        return Ok(orders);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await _orderApplicationService.CancelAsync(id, _currentUser.UserId, cancellationToken);
        return result.IsSuccess ? NoContent() : this.ToActionResult(result);
    }

}