namespace MediaHub.Domain.Entities;

public class MusicPlaylist
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}