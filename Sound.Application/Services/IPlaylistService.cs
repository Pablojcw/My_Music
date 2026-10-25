using Sound.Application.DTOs;

namespace Sound.Application.Services;

public interface IPlaylistService
{
    Task<IReadOnlyList<PlaylistResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<PlaylistResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<PlaylistResponseDto> CreateAsync(
        CreatePlaylistRequest request,
        CancellationToken cancellationToken);
    Task<PlaylistResponseDto?> UpdateAsync(
        int id,
        UpdatePlaylistRequest request,
        CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PlaylistMusicResponseDto>> GetMusicsAsync(
        int playlistId,
        CancellationToken cancellationToken);
    Task<PlaylistMusicResponseDto> AddMusicAsync(
        int playlistId,
        AddPlaylistMusicRequest request,
        CancellationToken cancellationToken);
    Task<PlaylistMusicResponseDto?> UpdateMusicPositionAsync(
        int playlistId,
        int musicId,
        UpdatePlaylistMusicPositionRequest request,
        CancellationToken cancellationToken);
    Task<bool> RemoveMusicAsync(
        int playlistId,
        int musicId,
        CancellationToken cancellationToken);
}
