using Amazon.S3;
using Amazon.S3.Model;
using MediaHub.Application.Services;
using Microsoft.Extensions.Options;

namespace MediaHub.Infrastructure.Storage;

public class R2StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public R2StorageService(IOptions<R2StorageOptions> options)
    {
        var config = options.Value;
        _bucketName = config.BucketName;

        var s3Config = new AmazonS3Config
        {
            ServiceURL = config.Endpoint,
            ForcePathStyle = true,
            RequestChecksumCalculation = Amazon.Runtime.RequestChecksumCalculation.WHEN_REQUIRED,
            ResponseChecksumValidation = Amazon.Runtime.ResponseChecksumValidation.WHEN_REQUIRED
        };

        _s3Client = new AmazonS3Client(config.AccessKeyId, config.SecretAccessKey, s3Config);
    }

    public async Task<string> UploadAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = content,
            ContentType = contentType,
            DisablePayloadSigning = true
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);
        return key;
    }

    public Task<string> GetPresignedUrlAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Expires = DateTime.UtcNow.Add(expiration)
        };

        var url = _s3Client.GetPreSignedURL(request);
        return Task.FromResult(url);
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };

        await _s3Client.DeleteObjectAsync(request, cancellationToken);
    }
}