using System.Globalization;
using Sound.Application.DTOs;
using Sound.Application.Exceptions;
using Sound.Application.Interfaces;
using Sound.Domain.Entities;

namespace Sound.Application.Services;

public sealed class AlbumService(IAlbumRepository albumRepository) : IAlbumService
{
    public async Task<IReadOnlyList<AlbumResponseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var albums = await albumRepository.GetAllAsync(cancellationToken);
        return albums.Select(Map).ToArray();
    }

    public async Task<AlbumResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var album = await albumRepository.GetByIdAsync(id, cancellationToken);
        return album is null ? null : Map(album);
    }

    public async Task<AlbumResponseDto> CreateAsync(
        CreateAlbumRequest request,
        CancellationToken cancellationToken)
    {
        await ValidateArtistAsync(request.ArtistId, cancellationToken);
        var album = CreateAlbum(request.Title, request.ReleaseDate, request.CoverUrl, request.ArtistId);
        return Map(await albumRepository.AddAsync(album, cancellationToken));
    }

    public async Task<AlbumResponseDto?> UpdateAsync(
        int id,
        UpdateAlbumRequest request,
        CancellationToken cancellationToken)
    {
        if (await albumRepository.GetByIdAsync(id, cancellationToken) is null)
        {
            return null;
        }

        await ValidateArtistAsync(request.ArtistId, cancellationToken);
        var updatedAlbum = CreateAlbum(
            request.Title,
            request.ReleaseDate,
            request.CoverUrl,
            request.ArtistId);

        var album = await albumRepository.UpdateAsync(id, updatedAlbum, cancellationToken);
        return album is null ? null : Map(album);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return albumRepository.DeleteAsync(id, cancellationToken);
    }

    private async Task ValidateArtistAsync(int artistId, CancellationToken cancellationToken)
    {
        if (!await albumRepository.ArtistExistsAsync(artistId, cancellationToken))
        {
            throw new InvalidAlbumReferenceException(
                "O código do artista deve corresponder a um artista cadastrado.");
        }
    }

    private static Album CreateAlbum(
        string title,
        DateTimeOffset releaseDate,
        string? coverUrl,
        int artistId)
    {
        return new Album
        {
            Title = title,
            ReleaseDate = releaseDate.ToString("O", CultureInfo.InvariantCulture),
            RealeaseDate = releaseDate.UtcDateTime,
            ConverUrl = coverUrl,
            ArtistId = artistId
        };
    }

    private static AlbumResponseDto Map(Album album)
    {
        return new AlbumResponseDto(
            album.Id,
            album.Title,
            new DateTimeOffset(DateTime.SpecifyKind(album.RealeaseDate, DateTimeKind.Utc)),
            album.ConverUrl,
            album.ArtistId,
            album.Artist.Name);
    }
}
