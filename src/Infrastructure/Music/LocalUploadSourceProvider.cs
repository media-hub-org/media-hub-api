using MediaHub.Application.Services;

namespace MediaHub.Infrastructure.Music;

public class LocalUploadSourceProvider : IMusicSourceProvider
{
    public string SourceName => "LocalUpload";
    public bool RequiresConversion => false;

    private readonly Dictionary<string, (Stream Stream, string FileName)> _pendingFiles = new();

    public void RegisterFile(string sourceInput, Stream stream, string fileName)
    {
        _pendingFiles[sourceInput] = (stream, fileName);
    }

    public Task<MusicSourceMetadata> GetMetadataAsync(string sourceInput, CancellationToken cancellationToken = default)
    {
        var (_, fileName) = _pendingFiles[sourceInput];
        var title = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrEmpty(extension))
        {
            extension = ".bin";
        }

        return Task.FromResult(new MusicSourceMetadata(title, null, null, null, extension));
    }

    public Task<Stream> GetRawStreamAsync(string sourceInput, CancellationToken cancellationToken = default)
    {
        var (stream, _) = _pendingFiles[sourceInput];
        return Task.FromResult(stream);
    }
}