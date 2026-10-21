namespace Sound.Domain.Entities;

public class LibraryMusic
{
    public int Id { get; set; }
    public int MusicId { get; set; }
    public Music Music { get; set; } = null!;
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}