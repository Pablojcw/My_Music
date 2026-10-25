using Microsoft.EntityFrameworkCore;
using Sound.Application.Exceptions;
using Sound.Application.Interfaces;
using Sound.Domain.Entities;
using SoundSpace.Infrastructure.Data;

namespace SoundSpace.Infrastructure.Repositories;

public sealed class MusicRepository(SoundSpaceDbContext dbContext) : IMusicRepository
{
    public async Task<IReadOnlyList<Music>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Musics
            .AsNoTracking()
            .Include(music => music.Artist)
            .Include(music => music.Album)
            .ToListAsync(cancellationToken);
    }

    public async Task<Music?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Musics
            .AsNoTracking()
            .Include(music => music.Artist)
            .Include(music => music.Album)
            .FirstOrDefaultAsync(music => music.Id == id, cancellationToken);
    }

    public async Task<Music?> GetByYouTubeVideoIdAsync(
        string videoId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Musics
            .AsNoTracking()
            .Include(music => music.Artist)
            .Include(music => music.Album)
            .FirstOrDefaultAsync(
                music => music.Source == MusicSource.YouTube &&
                    music.YouTubeVideoId == videoId,
                cancellationToken);
    }

    public async Task<LibraryMusic> SaveYouTubeVideoToLibraryAsync(
        Music music,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var savedMusic = await dbContext.Musics
            .FirstOrDefaultAsync(
                existing => existing.Source == MusicSource.YouTube &&
                    existing.YouTubeVideoId == music.YouTubeVideoId,
                cancellationToken);

        if (savedMusic is null)
        {
            savedMusic = music;
            dbContext.Musics.Add(savedMusic);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (await dbContext.LibraryMusic.AnyAsync(
                item => item.MusicId == savedMusic.Id,
                cancellationToken))
        {
            throw new ResourceConflictException("Este vídeo já está salvo na biblioteca.");
        }

        var libraryItem = new LibraryMusic
        {
            Music = savedMusic,
            SavedAt = DateTime.UtcNow
        };
        dbContext.LibraryMusic.Add(libraryItem);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return libraryItem;
    }

    public Task<bool> ArtistExistsAsync(int artistId, CancellationToken cancellationToken)
    {
        return dbContext.Artists
            .AnyAsync(artist => artist.ArtistId == artistId, cancellationToken);
    }

    public Task<bool> AlbumBelongsToArtistAsync(
        int albumId,
        int artistId,
        CancellationToken cancellationToken)
    {
        return dbContext.Albums
            .AnyAsync(
                album => album.Id == albumId && album.ArtistId == artistId,
                cancellationToken);
    }

    public async Task<Music> AddAsync(Music music, CancellationToken cancellationToken)
    {
        music.Artist = music.ArtistId is int artistId
            ? await dbContext.Artists.SingleAsync(
                artist => artist.ArtistId == artistId,
                cancellationToken)
            : null;
        music.Album = music.AlbumId is int albumId
            ? await dbContext.Albums.SingleAsync(album => album.Id == albumId, cancellationToken)
            : null;

        dbContext.Musics.Add(music);
        await dbContext.SaveChangesAsync(cancellationToken);
        return music;
    }

    public async Task<Music?> UpdateAsync(
        int id,
        Music updatedMusic,
        CancellationToken cancellationToken)
    {
        var music = await dbContext.Musics
            .FirstOrDefaultAsync(existing => existing.Id == id, cancellationToken);

        if (music is null)
        {
            return null;
        }

        music.Title = updatedMusic.Title;
        music.DurationSeconds = updatedMusic.DurationSeconds;
        music.AudioUrl = updatedMusic.AudioUrl;
        music.CoverUrl = updatedMusic.CoverUrl;
        music.ArtistId = updatedMusic.ArtistId;
        music.Artist = updatedMusic.ArtistId is int artistId
            ? await dbContext.Artists.SingleAsync(
                artist => artist.ArtistId == artistId,
                cancellationToken)
            : null;
        music.AlbumId = updatedMusic.AlbumId;
        music.Album = updatedMusic.AlbumId is int albumId
            ? await dbContext.Albums.SingleAsync(album => album.Id == albumId, cancellationToken)
            : null;
        music.Source = updatedMusic.Source;
        music.YouTubeVideoId = updatedMusic.YouTubeVideoId;
        music.ChannelTitle = updatedMusic.ChannelTitle;

        await dbContext.SaveChangesAsync(cancellationToken);
        return music;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var music = await dbContext.Musics
            .FirstOrDefaultAsync(existing => existing.Id == id, cancellationToken);

        if (music is null)
        {
            return false;
        }

        dbContext.Musics.Remove(music);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
