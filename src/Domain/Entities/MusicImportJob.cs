namespace MediaHub.Domain.Entities;

public enum MusicImportJobStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}

public class MusicImportJob
{
    public Guid Id { get; set; }
    public string SourceInput { get; set; } = string.Empty;
    public MusicImportJobStatus Status { get; set; } = MusicImportJobStatus.Pending;
    public Guid? MusicTrackId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}