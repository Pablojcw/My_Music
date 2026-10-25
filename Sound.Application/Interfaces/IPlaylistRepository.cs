using Sound.Domain.Entities;

namespace Sound.Application.Interfaces;

public interface IPlaylistRepository
{
    Task<IReadOnlyList<Playlist>> GetAllAsync(CancellationToken cancellationToken);
    Task<Playlist?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Playlist> AddAsync(Playlist playlist, CancellationToken cancellationToken);
    Task<Playlist?> UpdateAsync(int id, Playlist updatedPlaylist, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<int?> GetMusicCountAsync(int playlistId, CancellationToken cancellationToken);
    Task<bool> MusicExistsAsync(int musicId, CancellationToken cancellationToken);
    Task<bool> ContainsMusicAsync(int playlistId, int musicId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PlaylistMusic>> GetMusicsAsync(int playlistId, CancellationToken cancellationToken);
    Task<PlaylistMusic> AddMusicAsync(
        int playlistId,
        int musicId,
        int position,
        CancellationToken cancellationToken);
    Task<PlaylistMusic?> UpdateMusicPositionAsync(
        int playlistId,
        int musicId,
        int position,
        CancellationToken cancellationToken);
    Task<bool> RemoveMusicAsync(int playlistId, int musicId, CancellationToken cancellationToken);
}
