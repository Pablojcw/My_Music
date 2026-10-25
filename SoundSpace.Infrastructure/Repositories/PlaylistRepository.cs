using Microsoft.EntityFrameworkCore;
using Sound.Application.Interfaces;
using Sound.Domain.Entities;
using SoundSpace.Infrastructure.Data;

namespace SoundSpace.Infrastructure.Repositories;

public sealed class PlaylistRepository(SoundSpaceDbContext dbContext) : IPlaylistRepository
{
    public async Task<IReadOnlyList<Playlist>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Playlists
            .AsNoTracking()
            .OrderBy(playlist => playlist.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Playlist?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Playlists
            .AsNoTracking()
            .FirstOrDefaultAsync(playlist => playlist.Id == id, cancellationToken);
    }

    public async Task<Playlist> AddAsync(Playlist playlist, CancellationToken cancellationToken)
    {
        dbContext.Playlists.Add(playlist);
        await dbContext.SaveChangesAsync(cancellationToken);
        return playlist;
    }

    public async Task<Playlist?> UpdateAsync(
        int id,
        Playlist updatedPlaylist,
        CancellationToken cancellationToken)
    {
        var playlist = await dbContext.Playlists
            .FirstOrDefaultAsync(existing => existing.Id == id, cancellationToken);

        if (playlist is null)
        {
            return null;
        }

        playlist.Name = updatedPlaylist.Name;
        playlist.Description = updatedPlaylist.Description;
        playlist.CoverUrl = updatedPlaylist.CoverUrl;

        await dbContext.SaveChangesAsync(cancellationToken);
        return playlist;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var playlist = await dbContext.Playlists
            .FirstOrDefaultAsync(existing => existing.Id == id, cancellationToken);

        if (playlist is null)
        {
            return false;
        }

        dbContext.Playlists.Remove(playlist);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int?> GetMusicCountAsync(
        int playlistId,
        CancellationToken cancellationToken)
    {
        var playlistExists = await dbContext.Playlists
            .AnyAsync(playlist => playlist.Id == playlistId, cancellationToken);

        return playlistExists
            ? await dbContext.PlaylistMusic.CountAsync(
                item => item.PlaylistId == playlistId,
                cancellationToken)
            : null;
    }

    public Task<bool> MusicExistsAsync(int musicId, CancellationToken cancellationToken)
    {
        return dbContext.Musics.AnyAsync(music => music.Id == musicId, cancellationToken);
    }

    public Task<bool> ContainsMusicAsync(
        int playlistId,
        int musicId,
        CancellationToken cancellationToken)
    {
        return dbContext.PlaylistMusic.AnyAsync(
            item => item.PlaylistId == playlistId && item.MusicId == musicId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<PlaylistMusic>> GetMusicsAsync(
        int playlistId,
        CancellationToken cancellationToken)
    {
        return await dbContext.PlaylistMusic
            .AsNoTracking()
            .Where(item => item.PlaylistId == playlistId)
            .Include(item => item.Music)
                .ThenInclude(music => music.Artist)
            .Include(item => item.Music)
                .ThenInclude(music => music.Album)
            .OrderBy(item => item.Position)
            .ThenBy(item => item.AddedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PlaylistMusic> AddMusicAsync(
        int playlistId,
        int musicId,
        int position,
        CancellationToken cancellationToken)
    {
        var items = await dbContext.PlaylistMusic
            .Where(item => item.PlaylistId == playlistId)
            .OrderBy(item => item.Position)
            .ToListAsync(cancellationToken);

        foreach (var item in items.Where(item => item.Position >= position))
        {
            item.Position++;
        }

        var playlistMusic = new PlaylistMusic
        {
            PlaylistId = playlistId,
            MusicId = musicId,
            Position = position,
            AddedAt = DateTime.UtcNow
        };

        dbContext.PlaylistMusic.Add(playlistMusic);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.PlaylistMusic
            .AsNoTracking()
            .Where(item => item.PlaylistId == playlistId && item.MusicId == musicId)
            .Include(item => item.Music)
                .ThenInclude(music => music.Artist)
            .Include(item => item.Music)
                .ThenInclude(music => music.Album)
            .SingleAsync(cancellationToken);
    }

    public async Task<PlaylistMusic?> UpdateMusicPositionAsync(
        int playlistId,
        int musicId,
        int position,
        CancellationToken cancellationToken)
    {
        var items = await dbContext.PlaylistMusic
            .Where(item => item.PlaylistId == playlistId)
            .OrderBy(item => item.Position)
            .ToListAsync(cancellationToken);
        var currentIndex = items.FindIndex(item => item.MusicId == musicId);

        if (currentIndex < 0)
        {
            return null;
        }

        var target = items[currentIndex];
        items.RemoveAt(currentIndex);
        items.Insert(position, target);

        for (var index = 0; index < items.Count; index++)
        {
            items[index].Position = index;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return await dbContext.PlaylistMusic
            .AsNoTracking()
            .Where(item => item.PlaylistId == playlistId && item.MusicId == musicId)
            .Include(item => item.Music)
                .ThenInclude(music => music.Artist)
            .Include(item => item.Music)
                .ThenInclude(music => music.Album)
            .SingleAsync(cancellationToken);
    }

    public async Task<bool> RemoveMusicAsync(
        int playlistId,
        int musicId,
        CancellationToken cancellationToken)
    {
        var items = await dbContext.PlaylistMusic
            .Where(item => item.PlaylistId == playlistId)
            .OrderBy(item => item.Position)
            .ToListAsync(cancellationToken);
        var itemToRemove = items.FirstOrDefault(item => item.MusicId == musicId);

        if (itemToRemove is null)
        {
            return false;
        }

        dbContext.PlaylistMusic.Remove(itemToRemove);
        items.Remove(itemToRemove);

        for (var index = 0; index < items.Count; index++)
        {
            items[index].Position = index;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
