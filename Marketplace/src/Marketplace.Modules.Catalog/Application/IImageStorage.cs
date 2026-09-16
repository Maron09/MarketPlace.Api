namespace Marketplace.Modules.Catalog.Application
{
    internal interface IImageStorage
    {
        Task<string> UploadAsync(Stream content, string storageKey, string contentType, CancellationToken cancellationToken);
        Task DeleteAsync(string storageKey, CancellationToken cancellationToken);
        string GetPublicUrl(string storageKey);
    }
}