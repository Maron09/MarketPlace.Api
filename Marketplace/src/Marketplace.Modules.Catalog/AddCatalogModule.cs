using Amazon.S3;
using Marketplace.Modules.Catalog.Application;
using Marketplace.Modules.Catalog.Infrastructure;
using Marketplace.SharedKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace Marketplace.Modules.Catalog
{
    public static class CatalogModuleExtensions
    {
        public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IModuleDbContextConfigurator, CatalogDbContextConfigurator>();
            services.Configure<MinioSetings>(configuration.GetSection(MinioSetings.SectionName));

            services.AddSingleton<IAmazonS3>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MinioSetings>>().Value;
                var config = new AmazonS3Config
                {
                    ServiceURL = $"{(settings.UseSsl ? "https" : "http")}://{settings.Endpoint}",
                    ForcePathStyle = true // required for MinIO — it doesn't use virtual-hosted-style URLs like real S3
                };
                return new AmazonS3Client(settings.AccessKey, settings.SecretKey, config);
            });

            services.AddSingleton<IImageStorage, S3ImageStorage>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();

            services.AddScoped(sp => new CatalogApplicationService(
                sp.GetRequiredService<IProductRepository>(),
                sp.GetRequiredService<IProductImageRepository>(),
                sp.GetRequiredService<IImageStorage>()
            ));

            services.AddScoped(sp => new CategoryApplicationService(
                sp.GetRequiredService<ICategoryRepository>(),
                sp.GetRequiredService<IProductCategoryRepository>(),
                sp.GetRequiredService<IProductRepository>()
            ));

            return services;
        }
    }
}