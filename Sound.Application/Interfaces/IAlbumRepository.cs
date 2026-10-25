using Sound.Domain.Entities;

namespace Sound.Application.Interfaces;

public interface IAlbumRepository
{
    Task<IReadOnlyList<Album>> GetAllAsync(CancellationToken cancellationToken);
    Task<Album?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ArtistExistsAsync(int artistId, CancellationToken cancellationToken);
    Task<Album> AddAsync(Album album, CancellationToken cancellationToken);
    Task<Album?> UpdateAsync(int id, Album updatedAlbum, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
