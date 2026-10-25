using Sound.Application.DTOs;

namespace Sound.Application.Services;

public interface IArtistService
{
    Task<IReadOnlyList<ArtistResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<ArtistResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<ArtistResponseDto> CreateAsync(
        CreateArtistRequest request,
        CancellationToken cancellationToken);
    Task<ArtistResponseDto?> UpdateAsync(
        int id,
        UpdateArtistRequest request,
        CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
