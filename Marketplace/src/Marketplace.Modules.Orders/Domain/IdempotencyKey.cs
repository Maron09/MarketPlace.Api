namespace Marketplace.Modules.Orders.Domain;


public enum IdempotencyStatus
{
    Processing,
    Completed,
    Failed
}

public sealed class IdempotencyKey
{
    public Guid Key { get; private set; }
    public Guid UserId { get; private set; }
    public IdempotencyStatus Status { get; private set; }
    public Guid? OrderId { get; private set; }
    public string? FailureMessage { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private IdempotencyKey() { }

    public static IdempotencyKey CreateProcessing(Guid key, Guid userId)
    {
        return new IdempotencyKey
        {
            Key = key,
            UserId = userId,
            Status = IdempotencyStatus.Processing,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void MarkCompleted(Guid orderId)
    {
        Status = IdempotencyStatus.Completed;
        OrderId = orderId;
    }

    public void MarkFailed(string failureMessage)
    {
        Status = IdempotencyStatus.Failed;
        FailureMessage = failureMessage;
    }
}