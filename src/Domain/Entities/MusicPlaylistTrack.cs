namespace MediaHub.Domain.Entities;

public class MusicPlaylistTrack
{
    public Guid MusicPlaylistId { get; set; }
    public MusicPlaylist MusicPlaylist { get; set; } = null!;

    public Guid MusicTrackId { get; set; }
    public MusicTrack MusicTrack { get; set; } = null!;

    public int Order { get; set; }
}