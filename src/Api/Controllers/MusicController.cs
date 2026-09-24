using MediaHub.Application.Services;
using MediaHub.Domain.Entities;
using MediaHub.Infrastructure.Music;
using Microsoft.AspNetCore.Mvc;

namespace MediaHub.Api.Controllers;

[ApiController]
[Route("music")]
public class MusicController : ControllerBase
{
    private readonly LocalUploadSourceProvider _localUploadProvider;
    private readonly MusicIngestionService _ingestionService;
    private readonly IMusicTrackRepository _repository;
    private readonly IMusicImportJobRepository _jobRepository;
    private readonly IBackgroundJobQueue _jobQueue;

    public MusicController(
        LocalUploadSourceProvider localUploadProvider,
        MusicIngestionService ingestionService,
        IMusicTrackRepository repository,
        IMusicImportJobRepository jobRepository,
        IBackgroundJobQueue jobQueue)
    {
        _localUploadProvider = localUploadProvider;
        _ingestionService = ingestionService;
        _repository = repository;
        _jobRepository = jobRepository;
        _jobQueue = jobQueue;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("Nenhum arquivo enviado.");
        }

        var sourceInput = Guid.NewGuid().ToString();
        var stream = file.OpenReadStream();

        _localUploadProvider.RegisterFile(sourceInput, stream, file.FileName);

        var track = await _ingestionService.IngestAsync(_localUploadProvider, sourceInput, file.ContentType, cancellationToken);

        return Ok(track);
    }

    public record ImportYouTubeRequest(string Url);

    [HttpPost("import-youtube")]
    public async Task<IActionResult> ImportFromYouTube([FromBody] ImportYouTubeRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("URL não informada.");
        }

        var job = new MusicImportJob
        {
            Id = Guid.NewGuid(),
            SourceInput = request.Url,
            Status = MusicImportJobStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _jobRepository.AddAsync(job, cancellationToken);

        var jobId = job.Id;
        var url = request.Url;

        _jobQueue.QueueBackgroundWorkItem(async (serviceProvider, ct) =>
        {
            var jobRepo = serviceProvider.GetRequiredService<IMusicImportJobRepository>();
            var ingestionService = serviceProvider.GetRequiredService<MusicIngestionService>();
            var youTubeProvider = serviceProvider.GetRequiredService<YouTubeSourceProvider>();

            var currentJob = await jobRepo.GetByIdAsync(jobId, ct);
            if (currentJob is null)
            {
                return;
            }

            currentJob.Status = MusicImportJobStatus.Processing;
            await jobRepo.UpdateAsync(currentJob, ct);

            try
            {
                var track = await ingestionService.IngestAsync(youTubeProvider, url, "audio/mpeg", ct);

                currentJob.Status = MusicImportJobStatus.Completed;
                currentJob.MusicTrackId = track.Id;
                currentJob.CompletedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                currentJob.Status = MusicImportJobStatus.Failed;
                currentJob.ErrorMessage = ex.Message;
                currentJob.CompletedAt = DateTime.UtcNow;
            }

            await jobRepo.UpdateAsync(currentJob, ct);
        });

        return Accepted(new { jobId });
    }

    [HttpGet("jobs/{id:guid}")]
    public async Task<IActionResult> GetJobStatus(Guid id, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(id, cancellationToken);

        if (job is null)
        {
            return NotFound();
        }

        return Ok(job);
    }

    [HttpGet("library")]
    public async Task<IActionResult> GetLibrary(CancellationToken cancellationToken)
    {
        var tracks = await _repository.GetAllAsync(cancellationToken);
        return Ok(tracks);
    }
}