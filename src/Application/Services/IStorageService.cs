namespace MediaHub.Application.Services;

public interface IStorageService
{
    Task<string> UploadAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default);
    Task<string> GetPresignedUrlAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}