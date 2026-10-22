namespace Sound.Domain.Entities;

public class PlaylistMusic
{
    public int PlaylistId { get; set; }
    public Playlist Playlist { get; set; } = null!;

    public int MusicId { get; set; }
    public Music Music { get; set; } = null!;

    public int Position { get; set; }
    public DateTime AddedAt { get; set; }
}