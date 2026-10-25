namespace Sound.Domain.Entities;


public class Music
{
    public int Id { get; set;  }
    public string Title { get; set; } = string.Empty;
    public int? DurationSeconds { get; set; }
    public string? AudioUrl { get; set;  }
    public string? CoverUrl { get; set;  }
    public int? ArtistId { get; set; }
    public Artist? Artist { get; set; }
    public int? AlbumId { get; set; }
    public Album? Album { get; set; }
    public MusicSource Source { get; set; } = MusicSource.Catalog;
    public string? YouTubeVideoId { get; set; }
    public string? ChannelTitle { get; set; }
}