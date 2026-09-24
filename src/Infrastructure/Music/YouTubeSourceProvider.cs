using System.Diagnostics;
using System.Text.Json;
using MediaHub.Application.Services;

namespace MediaHub.Infrastructure.Music;

public class YouTubeSourceProvider : IMusicSourceProvider
{
    public string SourceName => "YouTube";
    public bool RequiresConversion => true;

    public async Task<MusicSourceMetadata> GetMetadataAsync(string sourceInput, CancellationToken cancellationToken = default)
    {
        var json = await RunProcessAsync("yt-dlp", new[] { "--dump-json", sourceInput }, cancellationToken);

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var title = root.TryGetProperty("title", out var titleProp) ? titleProp.GetString() ?? "Sem título" : "Sem título";
        var artist = root.TryGetProperty("uploader", out var uploaderProp) ? uploaderProp.GetString() : null;
        var duration = root.TryGetProperty("duration", out var durationProp) && durationProp.ValueKind == JsonValueKind.Number
            ? (int?)durationProp.GetDouble()
            : null;

        return new MusicSourceMetadata(title, artist, null, duration, ".mp3");
    }

    public async Task<Stream> GetRawStreamAsync(string sourceInput, CancellationToken cancellationToken = default)
    {
        var tempFileBase = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var outputTemplate = $"{tempFileBase}.%(ext)s";

        await RunProcessAsync(
            "yt-dlp",
            new[] { "-x", "--audio-format", "mp3", "-o", outputTemplate, sourceInput },
            cancellationToken);

        var mp3Path = $"{tempFileBase}.mp3";

        return new FileStream(mp3Path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.DeleteOnClose);
    }

    private static async Task<string> RunProcessAsync(string fileName, string[] arguments, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var arg in arguments)
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        var stdOutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stdErrTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var stdOut = await stdOutTask;
        var stdErr = await stdErrTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Falha ao executar '{fileName}': {stdErr}");
        }

        return stdOut;
    }
}