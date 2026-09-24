using MediaHub.Domain.Entities;

namespace MediaHub.Application.Services;

public interface IMusicImportJobRepository
{
    Task AddAsync(MusicImportJob job, CancellationToken cancellationToken = default);
    Task<MusicImportJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(MusicImportJob job, CancellationToken cancellationToken = default);
}