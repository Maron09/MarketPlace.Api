using Marketplace.SharedKernel;
using Marketplace.Modules.Catalog.Domain;


namespace Marketplace.Modules.Catalog.Application
{
    public sealed record CreateProductRequest(Guid VendorId, string Name, string? Description, decimal Price);
    public sealed record ProductDto(Guid Id, Guid VendorId, string Name, string? Description, decimal Price, bool IsActive);

    public sealed class CatalogApplicationService
    {
        private readonly IProductRepository _productRepository;

        internal CatalogApplicationService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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