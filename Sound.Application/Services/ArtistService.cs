using Sound.Application.DTOs;
using Sound.Application.Interfaces;
using Sound.Domain.Entities;

namespace Sound.Application.Services;

public sealed class ArtistService(IArtistRepository artistRepository) : IArtistService
{
    public async Task<IReadOnlyList<ArtistResponseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var artists = await artistRepository.GetAllAsync(cancellationToken);
        return artists.Select(Map).ToArray();
    }

    public async Task<ArtistResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var artist = await artistRepository.GetByIdAsync(id, cancellationToken);
        return artist is null ? null : Map(artist);
    }

    public async Task<ArtistResponseDto> CreateAsync(
        CreateArtistRequest request,
        CancellationToken cancellationToken)
    {
        var artist = new Artist
        {
            Name = request.Name,
            Biography = request.Biography,
            ImageUrl = request.ImageUrl
        };

        return Map(await artistRepository.AddAsync(artist, cancellationToken));
    }

    public async Task<ArtistResponseDto?> UpdateAsync(
        int id,
        UpdateArtistRequest request,
        CancellationToken cancellationToken)
    {
        var updatedArtist = new Artist
        {
            Name = request.Name,
            Biography = request.Biography,
            ImageUrl = request.ImageUrl
        };

        var artist = await artistRepository.UpdateAsync(id, updatedArtist, cancellationToken);
        return artist is null ? null : Map(artist);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return artistRepository.DeleteAsync(id, cancellationToken);
    }

    private static ArtistResponseDto Map(Artist artist)
    {
        return new ArtistResponseDto(
            artist.ArtistId,
            artist.Name,
            artist.Biography,
            artist.ImageUrl);
    }
}
