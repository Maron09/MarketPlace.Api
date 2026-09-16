using Marketplace.SharedKernel;
using Marketplace.Modules.Catalog.Domain;


namespace Marketplace.Modules.Catalog.Application
{
    public sealed record CreateProductRequest(Guid VendorId, string Name, string? Description, decimal Price);
    public sealed record ProductDto(Guid Id, Guid VendorId, string Name, string? Description, decimal Price, bool IsActive);
    


    public sealed class CatalogApplicationService
    {

        private static readonly Dictionary<string, string> ExtensionToContentType = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp"
        };

        private const long MaxImageSizeBytes = 5 * 1024 * 1024;
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IImageStorage _imageStorage;
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp" };

        internal CatalogApplicationService(IProductRepository productRepository, IProductImageRepository productImageRepository, IImageStorage imageStorage)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _imageStorage = imageStorage;
        }

        public async Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var product = Product.Create(request.VendorId, request.Name, request.Description, request.Price);
            await _productRepository.AddAsync(product, cancellationToken);
            await _productRepository.SaveChangesAsync(cancellationToken);
            return product.Id;
        }

        public async Task<Result<ProductDto>> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null)
                return Result<ProductDto>.Failure("Product not found.", ErrorType.NotFound);
            
            return Result<ProductDto>.Success(ToDto(product));
        }

        public async Task<IReadOnlyList<ProductDto>> BrowseAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetActiveAsync(page, pageSize, cancellationToken);
            return products.Select(ToDto).ToList();
        }

        public async Task<IReadOnlyList<ProductDto>> GetByVendorIdAsync(Guid vendorId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetByVendorIdAsync(vendorId, page, pageSize, cancellationToken);
            return products.Select(ToDto).ToList();
        }

        public async Task<Result> UpdateAsync(Guid productId, Guid requestingVendorId, string name, string? description, decimal price, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null)
                return Result.Failure("Product not found.", ErrorType.NotFound);
            
            if (product.VendorId != requestingVendorId)
                return Result.Failure("Product not found.", ErrorType.NotFound);
            
            var updateResult = product.UpdateDetails(name, description, price);
            if (!updateResult.IsSuccess)
                return updateResult;
            
            await _productRepository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeactivateAsync(Guid productId, Guid requestingVendorId, CancellationToken cancellationToken)
            => await ApplyOwnedTransitionAsync(productId, requestingVendorId, p => p.Deactivate(), cancellationToken);
        
        public async Task<Result> ReactivateAsync(Guid productId, Guid requestingVendorId, CancellationToken cancellationToken)
            => await ApplyOwnedTransitionAsync(productId, requestingVendorId, p => p.Reactivate(), cancellationToken);
        

        public async Task<Result<Guid>> AddImageAsync(
            Guid productId,
            Guid requestingVendorId,
            Stream content,
            string fileName,
            long contentLength,
            CancellationToken cancellationToken)
        {
            Console.WriteLine("Step 1: fetching product");
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null || product.VendorId != requestingVendorId)
                return Result<Guid>.Failure("Product not found.", ErrorType.NotFound);

            Console.WriteLine("Step 2: checking extension");
            Console.WriteLine($"fileName value: '{fileName}'");
            
            var extension = Path.GetExtension(fileName);
            if (!ExtensionToContentType.TryGetValue(extension, out var resolvedContentType))
                return Result<Guid>.Failure("Only JPEG, PNG, or WebP images are allowed.", ErrorType.Validation);

            Console.WriteLine($"Step 3: extension={extension}, resolvedContentType={resolvedContentType}");

            if (contentLength > MaxImageSizeBytes)
                return Result<Guid>.Failure("Image must be 5MB or smaller.", ErrorType.Validation);

            var imageId = Guid.NewGuid();
            var storageKey = $"products/{productId}/{imageId}{extension}";
            Console.WriteLine($"Step 4: storageKey={storageKey}, about to upload");

            await _imageStorage.UploadAsync(content, storageKey, resolvedContentType, cancellationToken);
            Console.WriteLine("Step 5: upload succeeded, getting display order");

            var displayOrder = await _productImageRepository.GetNextDisplayOrderAsync(productId, cancellationToken);
            Console.WriteLine($"Step 6: displayOrder={displayOrder}, creating entity");

            var image = new ProductImage(productId, storageKey, displayOrder);
            Console.WriteLine("Step 7: adding to repository");

            await _productImageRepository.AddAsync(image, cancellationToken);
            await _productImageRepository.SaveChangesAsync(cancellationToken);
            Console.WriteLine("Step 8: saved successfully");

            return Result<Guid>.Success(image.Id);
        }

        private async Task<Result> ApplyOwnedTransitionAsync(Guid productId,
        Guid requestingVendorId, Func<Domain.Product, Result> transition, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null || product.VendorId != requestingVendorId)
                return Result.Failure("Product not found.", ErrorType.NotFound);
            
            var result = transition(product);
            if (!result.IsSuccess)
                return result;
            await _productRepository.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        private static ProductDto ToDto(Product product)
            => new(product.Id, product.VendorId, product.Name, product.Description, product.Price, product.IsActive);
    }
}