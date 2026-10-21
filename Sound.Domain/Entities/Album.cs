namespace Sound.Domain.Entities
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ReleaseDate { get; set;  } = string.Empty;
        public DateTime RealeaseDate { get; set; }
        public string? ConverUrl { get; set; }
        public int ArtistId { get; set; }
        public Artist Artist { get; set; } = null!;
        public ICollection<Music> Musics { get; set; } = new List<Music>();

    }
}