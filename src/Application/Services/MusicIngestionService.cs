using MediaHub.Domain.Entities;

namespace MediaHub.Application.Services;

public class MusicIngestionService
{
    private readonly IStorageService _storageService;
    private readonly IMusicTrackRepository _repository;

    public MusicIngestionService(
        IStorageService storageService,
        IMusicTrackRepository repository)
    {
        _storageService = storageService;
        _repository = repository;
    }

    public async Task<MusicTrack> IngestAsync(
        IMusicSourceProvider sourceProvider,
        string sourceInput,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var metadata = await sourceProvider.GetMetadataAsync(sourceInput, cancellationToken);

        await using var rawStream = await sourceProvider.GetRawStreamAsync(sourceInput, cancellationToken);

        var storageKey = $"music/{Guid.NewGuid()}";

        await _storageService.UploadAsync(storageKey, rawStream, contentType, cancellationToken);

        var track = new MusicTrack
        {
            Id = Guid.NewGuid(),
            Title = metadata.Title,
            Artist = metadata.Artist,
            Album = metadata.Album,
            DurationSeconds = metadata.DurationSeconds,
            SourceName = sourceProvider.SourceName,
            SourceUrl = sourceProvider.SourceName == "YouTube" ? sourceInput : null,
            StorageKey = storageKey,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(track, cancellationToken);

        return track;
    }
}