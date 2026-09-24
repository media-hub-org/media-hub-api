using MediaHub.Domain.Entities;

namespace MediaHub.Application.Services;

public interface IMusicTrackRepository
{
    Task AddAsync(MusicTrack track, CancellationToken cancellationToken = default);
    Task<List<MusicTrack>> GetAllAsync(CancellationToken cancellationToken = default);
}