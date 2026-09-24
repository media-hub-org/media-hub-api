using MediaHub.Application.Services;
using MediaHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaHub.Infrastructure.Persistence;

public class MusicTrackRepository : IMusicTrackRepository
{
    private readonly AppDbContext _context;

    public MusicTrackRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(MusicTrack track, CancellationToken cancellationToken = default)
    {
        _context.MusicTracks.Add(track);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<MusicTrack>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MusicTracks
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}