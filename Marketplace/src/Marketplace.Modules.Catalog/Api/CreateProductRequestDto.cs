namespace Marketplace.Modules.Catalog.Api
{
    public sealed record CreateProductRequestDto(string Name, string? Description, decimal Price);
}