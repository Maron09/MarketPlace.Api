namespace Marketplace.Modules.Catalog.Api
{
    public sealed record CreateProductRequestDto(Guid VendorId, string Name, string? Description, decimal Price);
}