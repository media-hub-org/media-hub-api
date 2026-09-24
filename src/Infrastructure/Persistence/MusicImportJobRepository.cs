using MediaHub.Application.Services;
using MediaHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaHub.Infrastructure.Persistence;

public class MusicImportJobRepository : IMusicImportJobRepository
{
    private readonly AppDbContext _context;

    public MusicImportJobRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(MusicImportJob job, CancellationToken cancellationToken = default)
    {
        _context.MusicImportJobs.Add(job);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<MusicImportJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MusicImportJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(MusicImportJob job, CancellationToken cancellationToken = default)
    {
        _context.MusicImportJobs.Update(job);
        await _context.SaveChangesAsync(cancellationToken);
    }
}