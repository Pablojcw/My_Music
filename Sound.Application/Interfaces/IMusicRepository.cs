using Sound.Domain.Entities;

namespace Sound.Application.Interfaces;

public interface IMusicRepository
{
    Task<IReadOnlyList<Music>> GetAllAsync(CancellationToken cancellationToken);
    Task<Music?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ArtistExistsAsync(int artistId, CancellationToken cancellationToken);
    Task<bool> AlbumBelongsToArtistAsync(
        int albumId,
        int artistId,
        CancellationToken cancellationToken);
    Task<Music> AddAsync(Music music, CancellationToken cancellationToken);
    Task<Music?> UpdateAsync(int id, Music updatedMusic, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
