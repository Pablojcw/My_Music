using Microsoft.EntityFrameworkCore;
using Sound.Application.Interfaces;
using Sound.Domain.Entities;
using SoundSpace.Infrastructure.Data;

namespace SoundSpace.Infrastructure.Repositories;

public sealed class AlbumRepository(SoundSpaceDbContext dbContext) : IAlbumRepository
{
    public async Task<IReadOnlyList<Album>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Albums
            .AsNoTracking()
            .Include(album => album.Artist)
            .ToListAsync(cancellationToken);
    }

    public async Task<Album?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Albums
            .AsNoTracking()
            .Include(album => album.Artist)
            .FirstOrDefaultAsync(album => album.Id == id, cancellationToken);
    }

    public Task<bool> ArtistExistsAsync(int artistId, CancellationToken cancellationToken)
    {
        return dbContext.Artists
            .AnyAsync(artist => artist.ArtistId == artistId, cancellationToken);
    }

    public async Task<Album> AddAsync(Album album, CancellationToken cancellationToken)
    {
        album.Artist = await dbContext.Artists
            .SingleAsync(artist => artist.ArtistId == album.ArtistId, cancellationToken);

        dbContext.Albums.Add(album);
        await dbContext.SaveChangesAsync(cancellationToken);
        return album;
    }

    public async Task<Album?> UpdateAsync(
        int id,
        Album updatedAlbum,
        CancellationToken cancellationToken)
    {
        var album = await dbContext.Albums
            .Include(existing => existing.Artist)
            .FirstOrDefaultAsync(existing => existing.Id == id, cancellationToken);

        if (album is null)
        {
            return null;
        }

        album.Title = updatedAlbum.Title;
        album.ReleaseDate = updatedAlbum.ReleaseDate;
        album.RealeaseDate = updatedAlbum.RealeaseDate;
        album.ConverUrl = updatedAlbum.ConverUrl;
        album.ArtistId = updatedAlbum.ArtistId;
        album.Artist = await dbContext.Artists
            .SingleAsync(artist => artist.ArtistId == updatedAlbum.ArtistId, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        return album;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var album = await dbContext.Albums
            .FirstOrDefaultAsync(existing => existing.Id == id, cancellationToken);

        if (album is null)
        {
            return false;
        }

        var musics = await dbContext.Musics
            .Where(music => music.AlbumId == id)
            .ToListAsync(cancellationToken);

        foreach (var music in musics)
        {
            music.AlbumId = null;
            music.Album = null;
        }

        dbContext.Albums.Remove(album);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
