namespace Sound.Domain.Entities;

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
    public DateTime CreatAt { get; set; } = DateTime.UtcNow;

    public ICollection<PlaylistMusic> PlaylistMusics { get; set; }
        = new List<PlaylistMusic>();


}