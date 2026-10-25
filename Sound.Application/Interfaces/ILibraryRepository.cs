using Sound.Domain.Entities;

namespace Sound.Application.Interfaces;

public interface ILibraryRepository
{
    Task<IReadOnlyList<LibraryMusic>> GetAllAsync(CancellationToken cancellationToken);
    Task<LibraryMusic?> GetByMusicIdAsync(int musicId, CancellationToken cancellationToken);
    Task<bool> MusicExistsAsync(int musicId, CancellationToken cancellationToken);
    Task<bool> ContainsMusicAsync(int musicId, CancellationToken cancellationToken);
    Task<LibraryMusic> AddAsync(int musicId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int musicId, CancellationToken cancellationToken);
}
