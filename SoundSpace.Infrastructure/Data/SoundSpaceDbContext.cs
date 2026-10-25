namespace SoundSpace.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Sound.Domain.Entities;
public class SoundSpaceDbContext : DbContext
{

    public SoundSpaceDbContext(DbContextOptions<SoundSpaceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Music> Musics => Set<Music>();
    public DbSet<Artist> Artists => Set<Artist>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<LibraryMusic> LibraryMusic => Set<LibraryMusic>();
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<PlaylistMusic> PlaylistMusic => Set<PlaylistMusic>();

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<PlaylistMusic>()
        .HasKey(pm => new { pm.PlaylistId, pm.MusicId });

    modelBuilder.Entity<Music>()
        .HasIndex(music => music.YouTubeVideoId)
        .IsUnique()
        .HasFilter("\"YouTubeVideoId\" IS NOT NULL");
    modelBuilder.Entity<Music>()
        .Property(music => music.Source)
        .HasDefaultValue(MusicSource.Catalog);

    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(SoundSpaceDbContext).Assembly);

    base.OnModelCreating(modelBuilder);
}

}
