namespace MediaHub.Infrastructure.Storage;

public class R2StorageOptions
{
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
}