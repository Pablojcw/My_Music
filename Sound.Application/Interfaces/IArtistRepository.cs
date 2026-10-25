using Sound.Domain.Entities;

namespace Sound.Application.Interfaces;

public interface IArtistRepository
{
    Task<IReadOnlyList<Artist>> GetAllAsync(CancellationToken cancellationToken);
    Task<Artist?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Artist> AddAsync(Artist artist, CancellationToken cancellationToken);
    Task<Artist?> UpdateAsync(int id, Artist updatedArtist, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
