namespace MediaHub.Domain.Entities;

public class MusicTrackTag
{
    public Guid MusicTrackId { get; set; }
    public MusicTrack MusicTrack { get; set; } = null!;

    public Guid MusicTagId { get; set; }
    public MusicTag MusicTag { get; set; } = null!;
}