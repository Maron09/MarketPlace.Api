namespace Marketplace.Modules.Identity.Infrastructure
{
    internal sealed class JwtSettings
    {
        public const string SectionName = "Jwt";

        public string SigningKey { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int AccessTokenLifetimeMinutes { get; init; } = 15;
    }
}