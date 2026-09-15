using Marketplace.SharedKernel;
using Marketplace.Modules.Catalog.Domain;


namespace Marketplace.Modules.Catalog.Application
{
    public sealed record CategoryDto(Guid Id, string Name);

    public sealed class CategoryApplicationService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IProductRepository _productRepository;

        internal CategoryApplicationService(ICategoryRepository categoryRepository, IProductCategoryRepository productCategoryRepository, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _productRepository = productRepository;
        }

        public async Task<Result<Guid>> CreateAsync(string name, CancellationToken cancellationToken)
        {
            if (await _categoryRepository.NameExistsAsync(name.Trim(), excludingCategoryId: null, cancellationToken))
                return Result<Guid>.Failure("A category with this name already exists.", ErrorType.Conflict);
            
            var category = Category.Create(name);
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(category.Id);
        }

        public async Task<Result> RenameAsync(Guid categoryId, string newName, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category is null)
                return Result.Failure("Category not found.", ErrorType.NotFound);
            if (await _categoryRepository.NameExistsAsync(newName.Trim(), excludingCategoryId: categoryId, cancellationToken))
                return Result.Failure("A category with this name already exists.", ErrorType.Conflict);
            category.Rename(newName);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> AssignToProductAsync(Guid productId, Guid categoryId, Guid requestingVendorId, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null || product.VendorId != requestingVendorId)
                return Result.Failure("Product not found.", ErrorType.NotFound);
            
            var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category is null)
                return Result.Failure("Category not found.", ErrorType.NotFound);
            
            if (await _productCategoryRepository.ExistsAsync(productId, categoryId, cancellationToken))
                return Result.Failure("Product is already assigned to this category.", ErrorType.Conflict);
            
            await _productCategoryRepository.AddAsync(productId, categoryId, cancellationToken);
            await _productCategoryRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> RemoveFromProductAsync(Guid productId, Guid categoryId, Guid requestingVendorId, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null || product.VendorId != requestingVendorId)
            {
                return Result.Failure("Product not found.", ErrorType.NotFound);
            }
            if (!await _productCategoryRepository.ExistsAsync(productId, categoryId, cancellationToken))
                return Result.Failure("Product is not assigned to this category.", ErrorType.NotFound);
            
            await _productCategoryRepository.RemoveAsync(productId, categoryId, cancellationToken);
            await _productCategoryRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync(cancellationToken);
            return [.. categories.Select(c => new CategoryDto(c.Id, c.Name))];
        }
    }
}