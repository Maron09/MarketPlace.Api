using Marketplace.Modules.Identity.Application;

namespace Marketplace.Modules.Identity.Domain
{
    internal interface ITokenGenerator
    {
        string GenerateAccessToken(Guid userId, UserRole role);
    }
}