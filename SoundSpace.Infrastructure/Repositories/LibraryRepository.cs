using Microsoft.EntityFrameworkCore;
using Sound.Application.Interfaces;
using Sound.Domain.Entities;
using SoundSpace.Infrastructure.Data;

namespace SoundSpace.Infrastructure.Repositories;

public sealed class LibraryRepository(SoundSpaceDbContext dbContext) : ILibraryRepository
{
    public async Task<IReadOnlyList<LibraryMusic>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.LibraryMusic
            .AsNoTracking()
            .Include(item => item.Music)
                .ThenInclude(music => music.Artist)
            .Include(item => item.Music)
                .ThenInclude(music => music.Album)
            .OrderByDescending(item => item.SavedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<LibraryMusic?> GetByMusicIdAsync(
        int musicId,
        CancellationToken cancellationToken)
    {
        return await dbContext.LibraryMusic
            .AsNoTracking()
            .Where(item => item.MusicId == musicId)
            .Include(item => item.Music)
                .ThenInclude(music => music.Artist)
            .Include(item => item.Music)
                .ThenInclude(music => music.Album)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> MusicExistsAsync(int musicId, CancellationToken cancellationToken)
    {
        return dbContext.Musics.AnyAsync(music => music.Id == musicId, cancellationToken);
    }

    public Task<bool> ContainsMusicAsync(int musicId, CancellationToken cancellationToken)
    {
        return dbContext.LibraryMusic
            .AnyAsync(item => item.MusicId == musicId, cancellationToken);
    }

    public async Task<LibraryMusic> AddAsync(int musicId, CancellationToken cancellationToken)
    {
        var item = new LibraryMusic
        {
            MusicId = musicId,
            SavedAt = DateTime.UtcNow
        };

        dbContext.LibraryMusic.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.LibraryMusic
            .AsNoTracking()
            .Where(saved => saved.Id == item.Id)
            .Include(saved => saved.Music)
                .ThenInclude(music => music.Artist)
            .Include(saved => saved.Music)
                .ThenInclude(music => music.Album)
            .SingleAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int musicId, CancellationToken cancellationToken)
    {
        var item = await dbContext.LibraryMusic
            .FirstOrDefaultAsync(saved => saved.MusicId == musicId, cancellationToken);

        if (item is null)
        {
            return false;
        }

        dbContext.LibraryMusic.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
