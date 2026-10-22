using Sound.Application.DTOs;

namespace Sound.Application.Services;

public interface IMusicService
{
    Task<IReadOnlyList<MusicResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<MusicResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<MusicResponseDto> CreateAsync(
        CreateMusicRequest request,
        CancellationToken cancellationToken);
    Task<MusicResponseDto?> UpdateAsync(
        int id,
        UpdateMusicRequest request,
        CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
