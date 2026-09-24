using MediaHub.Application.Services;
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

    public MusicController(
        LocalUploadSourceProvider localUploadProvider,
        MusicIngestionService ingestionService,
        IMusicTrackRepository repository)
    {
        _localUploadProvider = localUploadProvider;
        _ingestionService = ingestionService;
        _repository = repository;
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

        var track = await _ingestionService.IngestAsync(sourceInput, file.ContentType, cancellationToken);

        return Ok(track);
    }

    [HttpGet("library")]
    public async Task<IActionResult> GetLibrary(CancellationToken cancellationToken)
    {
        var tracks = await _repository.GetAllAsync(cancellationToken);
        return Ok(tracks);
    }
}