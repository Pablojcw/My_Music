using Microsoft.EntityFrameworkCore;
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
        music.Artist = await dbContext.Artists
            .SingleAsync(artist => artist.ArtistId == music.ArtistId, cancellationToken);
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
        music.Artist = await dbContext.Artists
            .SingleAsync(artist => artist.ArtistId == updatedMusic.ArtistId, cancellationToken);
        music.AlbumId = updatedMusic.AlbumId;
        music.Album = updatedMusic.AlbumId is int albumId
            ? await dbContext.Albums.SingleAsync(album => album.Id == albumId, cancellationToken)
            : null;

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
