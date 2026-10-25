using Microsoft.EntityFrameworkCore;
using Sound.Application.Interfaces;
using Sound.Domain.Entities;
using SoundSpace.Infrastructure.Data;

namespace SoundSpace.Infrastructure.Repositories;

public sealed class ArtistRepository(SoundSpaceDbContext dbContext) : IArtistRepository
{
    public async Task<IReadOnlyList<Artist>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Artists
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Artist?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Artists
            .AsNoTracking()
            .FirstOrDefaultAsync(artist => artist.ArtistId == id, cancellationToken);
    }

    public async Task<Artist> AddAsync(Artist artist, CancellationToken cancellationToken)
    {
        dbContext.Artists.Add(artist);
        await dbContext.SaveChangesAsync(cancellationToken);
        return artist;
    }

    public async Task<Artist?> UpdateAsync(
        int id,
        Artist updatedArtist,
        CancellationToken cancellationToken)
    {
        var artist = await dbContext.Artists
            .FirstOrDefaultAsync(existing => existing.ArtistId == id, cancellationToken);

        if (artist is null)
        {
            return null;
        }

        artist.Name = updatedArtist.Name;
        artist.Biography = updatedArtist.Biography;
        artist.ImageUrl = updatedArtist.ImageUrl;

        await dbContext.SaveChangesAsync(cancellationToken);
        return artist;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var artist = await dbContext.Artists
            .FirstOrDefaultAsync(existing => existing.ArtistId == id, cancellationToken);

        if (artist is null)
        {
            return false;
        }

        dbContext.Artists.Remove(artist);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}