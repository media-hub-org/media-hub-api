namespace MediaHub.Application.Services;

public record MusicSourceMetadata(
    string Title,
    string? Artist,
    string? Album,
    int? DurationSeconds,
    string FileExtension
);

public interface IMusicSourceProvider
{
    string SourceName { get; }
    bool RequiresConversion { get; }

    Task<MusicSourceMetadata> GetMetadataAsync(string sourceInput, CancellationToken cancellationToken = default);
    Task<Stream> GetRawStreamAsync(string sourceInput, CancellationToken cancellationToken = default);
}