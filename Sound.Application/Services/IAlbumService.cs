using Sound.Application.DTOs;

namespace Sound.Application.Services;

public interface IAlbumService
{
    Task<IReadOnlyList<AlbumResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<AlbumResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<AlbumResponseDto> CreateAsync(
        CreateAlbumRequest request,
        CancellationToken cancellationToken);
    Task<AlbumResponseDto?> UpdateAsync(
        int id,
        UpdateAlbumRequest request,
        CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
