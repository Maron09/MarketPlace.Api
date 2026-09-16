namespace Marketplace.Modules.Catalog.Infrastructure
{
    public sealed class MinioSetings
    {
        public const string SectionName = "Minio";

        public string Endpoint { get; init; } = string.Empty;
        public string AccessKey { get; init; } = string.Empty;
        public string SecretKey { get; init; } = string.Empty;
        public string BucketName { get; init; } = string.Empty;
        public bool UseSsl { get; init; } = false;
    }
}