using Marketplace.Modules.Cart.Contracts;
using Marketplace.Modules.Catalog.Contracts;
using Marketplace.Modules.Inventory.Contracts;
using Marketplace.Modules.Orders.Domain;
using Marketplace.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Marketplace.Modules.Orders.Application;

public sealed record OrderItemDto(Guid ProductId, Guid VendorId, string ProductName, decimal UnitPrice, int Quantity);
public sealed record OrderDto(Guid Id, Guid UserId, string Status, decimal TotalAmount, DateTime CreatedAtUtc, IReadOnlyList<OrderItemDto> Items);


public sealed class OrderApplicationService
{
    private const int MaxAttempts = 3;
    private const string DuplicateIdempotencyKeyMessage = "A request with this idempotency key is already being processed.";

    private readonly IOrderRepository _orderRepository;
    private readonly ICartReader _cartReader;
    private readonly IProductCatalogReader _productCatalogReader;
    private readonly IStockReservation _stockReservation;
    private readonly IStockRelease _stockRelease;
    private readonly IIdempotencyKeyRepository _idempotencyKeyRepository;
    private readonly ILogger<OrderApplicationService> _logger;

    internal OrderApplicationService(
        IOrderRepository orderRepository,
        ICartReader cartReader,
        IProductCatalogReader productCatalogReader,
        IStockReservation stockReservation,
        IStockRelease stockRelease,
        IIdempotencyKeyRepository idempotencyKeyRepository,
        ILogger<OrderApplicationService> logger)
    {
        _orderRepository = orderRepository;
        _cartReader = cartReader;
        _productCatalogReader = productCatalogReader;
        _stockReservation = stockReservation;
        _stockRelease = stockRelease;
        _idempotencyKeyRepository = idempotencyKeyRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> PlaceOrderAsync(Guid userId, Guid idempotencyKey, CancellationToken cancellationToken)
    {
        var existing = await _idempotencyKeyRepository.FindAsync(idempotencyKey, cancellationToken);
        if (existing is not null)
        {
            return ToResultFromExisting(existing);
        }

        var claim = IdempotencyKey.CreateProcessing(idempotencyKey, userId);
        await _idempotencyKeyRepository.AddAsync(claim, cancellationToken);

        try
        {
            await _idempotencyKeyRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            // Someone else claimed this key in the gap between our check and insert.
            var winner = await _idempotencyKeyRepository.FindAsync(idempotencyKey, cancellationToken);
            return winner is not null
                ? ToResultFromExisting(winner)
                : Result<Guid>.Failure(DuplicateIdempotencyKeyMessage, ErrorType.Conflict);
        }

        var result = await PlaceOrderCoreAsync(userId, cancellationToken);

        if (result.IsSuccess)
        {
            claim.MarkCompleted(result.Value);
        }
        else
        {
            claim.MarkFailed(result.Error);
        }

        await _idempotencyKeyRepository.SaveChangesAsync(cancellationToken);

        return result;
    }

    private static Result<Guid> ToResultFromExisting(IdempotencyKey existing)
    {
        return existing.Status switch
        {
            IdempotencyStatus.Completed => Result<Guid>.Success(existing.OrderId!.Value),
            IdempotencyStatus.Failed => Result<Guid>.Failure(existing.FailureMessage!, ErrorType.Conflict),
            _ => Result<Guid>.Failure(DuplicateIdempotencyKeyMessage, ErrorType.Conflict)
        };
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
        => ex.InnerException is PostgresException pgEx && pgEx.SqlState == PostgresErrorCodes.UniqueViolation;

    private async Task<Result<Guid>> PlaceOrderCoreAsync(Guid userId, CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var cartLines = await _cartReader.GetCartLinesAsync(userId, cancellationToken);
            if (cartLines.Count == 0)
            {
                return Result<Guid>.Failure("Cart is empty.", ErrorType.Validation);
            }
            var order = Order.Create(userId);

            foreach(var line in cartLines)
            {
                var product = await _productCatalogReader.GetSnapshotAsync(line.ProductId, cancellationToken);
                if (product is null || !product.IsActive)
                {
                    return Result<Guid>.Failure($"Product {line.ProductId} is no longer available.", ErrorType.Conflict);
                }
                if (product.Price != line.SnapshottedUnitPrice)
                {
                    return Result<Guid>.Failure(
                        $"Price for {product.Name} has changed since it was added to your cart. Please review your cart.",
                        ErrorType.Conflict);
                }

                var reserved = await _stockReservation.TryReserveAsync(line.ProductId, line.Quantity, cancellationToken);
                if (!reserved)
                {
                    return Result<Guid>.Failure($"Insufficient stock for product {product.Name}.", ErrorType.Conflict);
                }
                order.AddItem(line.ProductId, product.VendorId, product.Name, product.Price, line.Quantity);
            }

            await _orderRepository.AddAsync(order, cancellationToken);
            await _cartReader.ClearCartAsync(userId, cancellationToken);

            try
            {
                await _orderRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Order {OrderId} placed successfully for user {UserId}.", order.Id, userId);
                return Result<Guid>.Success(order.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning(
                    "Concurrency conflict placing order for user {UserId}, attempt {Attempt}/{MaxAttempts}",
                    userId, attempt, MaxAttempts);
            }
        }
        return Result<Guid>.Failure("Could not place order due to high demand. Please try again.", ErrorType.Conflict);
    }
    public async Task<Result<OrderDto>> GetByIdAsync(Guid orderId, Guid requestingUserId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null || order.UserId != requestingUserId)
        {
            return Result<OrderDto>.Failure("Order not found.", ErrorType.NotFound);
        }

        return Result<OrderDto>.Success(ToDto(order));
    }

    public async Task<IReadOnlyList<OrderDto>> GetMyOrdersAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, page, pageSize, cancellationToken);
        return [.. orders.Select(ToDto)];
    }

    public async Task<Result> CancelAsync(Guid orderId, Guid requestingUserId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null || order.UserId != requestingUserId)
        {
            return Result.Failure("Order not found.", ErrorType.NotFound);
        }

        var cancelResult = order.Cancel();
        if (!cancelResult.IsSuccess)
        {
            return cancelResult;
        }

        foreach (var item in order.Items)
        {
            await _stockRelease.ReleaseAsync(item.ProductId, item.Quantity, cancellationToken);
        }

        await _orderRepository.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Order {OrderId} cancelled by user {UserId}", orderId, requestingUserId);
        return Result.Success();
    }


    private static OrderDto ToDto(Order order)
    {
        var items = order.Items
            .Select(i => new OrderItemDto(i.ProductId, i.VendorId, i.ProductNameSnapshot, i.UnitPriceSnapshot, i.Quantity))
            .ToList();

        return new OrderDto(order.Id, order.UserId, order.Status.ToString(), order.TotalAmount, order.CreatedAtUtc, items);
    }
}