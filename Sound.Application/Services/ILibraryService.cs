using Sound.Application.DTOs;

namespace Sound.Application.Services;

public interface ILibraryService
{
    Task<IReadOnlyList<LibraryMusicResponseDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<LibraryMusicResponseDto?> GetByMusicIdAsync(
        int musicId,
        CancellationToken cancellationToken);
    Task<LibraryMusicResponseDto> AddAsync(
        AddLibraryMusicRequest request,
        CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int musicId, CancellationToken cancellationToken);
}
