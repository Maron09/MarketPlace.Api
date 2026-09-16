using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;


namespace Marketplace.Modules.Catalog.Infrastructure
{
    public static class BucketInitializer
    {
        public static async Task EnsureBucketExistsAsync(IAmazonS3 s3Client, IOptions<MinioSetings> settings)
        {
            var bucketName = settings.Value.BucketName;

            var exists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName);
            
            if (!exists)
            {
                await s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                });
                
            }
        }
    }
}