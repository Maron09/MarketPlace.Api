namespace Marketplace.SharedKernel;

public interface ICurrentUser
{
    Guid UserId { get; }
}