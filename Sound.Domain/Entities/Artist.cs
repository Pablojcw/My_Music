namespace Sound.Domain.Entities;

public class Artist
{
    public int ArtistId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set;  } 
    public string? ImageUrl { get; set;  }
    public ICollection<Album> Albums { get; set; } = new List<Album>();
    public ICollection<Music> Musics { get; set; } = new List<Music>();
}