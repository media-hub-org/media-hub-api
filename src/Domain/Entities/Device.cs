namespace MediaHub.Domain.Entities;

public class Device
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastAccessAt { get; set; }
    public bool IsActive { get; set; } = true;
}