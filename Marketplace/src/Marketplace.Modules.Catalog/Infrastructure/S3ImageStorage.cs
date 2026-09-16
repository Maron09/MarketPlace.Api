using Amazon.S3;
using Amazon.S3.Model;
using Marketplace.Modules.Catalog.Application;
using Microsoft.Extensions.Options;


namespace Marketplace.Modules.Catalog.Infrastructure
{
    internal sealed class S3ImageStorage : IImageStorage
    {
        private readonly IAmazonS3 _s3Client;
        private readonly MinioSetings _settings;


        public S3ImageStorage(IAmazonS3 s3Client, IOptions<MinioSetings> settings)
        {
            _s3Client = s3Client;
            _settings = settings.Value;
        }

        public async Task<string> UploadAsync(Stream content, string storageKey, string contentType, CancellationToken cancellationToken)
        {
            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = storageKey,
                InputStream = content,
                ContentType = contentType
            };

            await _s3Client.PutObjectAsync(request, cancellationToken);
            return storageKey;
        }

        public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
        {
            await _s3Client.DeleteObjectAsync(_settings.BucketName, storageKey, cancellationToken);
        }

        public string GetPublicUrl(string storageKey)
        {
            var scheme = _settings.UseSsl ? "https" : "http";
            return $"{scheme}://{_settings.Endpoint}/{_settings.BucketName}/{storageKey}";
        }
    }
}