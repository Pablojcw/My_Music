using Sound.Application.DTOs;
using Sound.Application.Exceptions;
using Sound.Application.Interfaces;
using Sound.Domain.Entities;

namespace Sound.Application.Services;

public sealed class MusicService(IMusicRepository musicRepository) : IMusicService
{
    public async Task<IReadOnlyList<MusicResponseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var musics = await musicRepository.GetAllAsync(cancellationToken);

        return musics.Select(Map).ToArray();
    }

    public async Task<MusicResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var music = await musicRepository.GetByIdAsync(id, cancellationToken);
        return music is null ? null : Map(music);
    }

    public async Task<MusicResponseDto> CreateAsync(
        CreateMusicRequest request,
        CancellationToken cancellationToken)
    {
        await ValidateReferencesAsync(request.ArtistId, request.AlbumId, cancellationToken);
        var music = new Music
        {
            Title = request.Title,
            DurationSeconds = request.DurationSeconds,
            AudioUrl = request.AudioUrl,
            CoverUrl = request.CoverUrl,
            ArtistId = request.ArtistId,
            AlbumId = request.AlbumId,
            Source = MusicSource.Catalog
        };

        return Map(await musicRepository.AddAsync(music, cancellationToken));
    }

    public async Task<MusicResponseDto?> UpdateAsync(
        int id,
        UpdateMusicRequest request,
        CancellationToken cancellationToken)
    {
        if (await musicRepository.GetByIdAsync(id, cancellationToken) is null)
        {
            return null;
        }

        await ValidateReferencesAsync(request.ArtistId, request.AlbumId, cancellationToken);
        var updatedMusic = new Music
        {
            Title = request.Title,
            DurationSeconds = request.DurationSeconds,
            AudioUrl = request.AudioUrl,
            CoverUrl = request.CoverUrl,
            ArtistId = request.ArtistId,
            AlbumId = request.AlbumId,
            Source = MusicSource.Catalog
        };

        var music = await musicRepository.UpdateAsync(id, updatedMusic, cancellationToken);
        return music is null ? null : Map(music);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return musicRepository.DeleteAsync(id, cancellationToken);
    }

    private async Task ValidateReferencesAsync(
        int artistId,
        int? albumId,
        CancellationToken cancellationToken)
    {
        if (!await musicRepository.ArtistExistsAsync(artistId, cancellationToken))
        {
            throw new InvalidMusicReferenceException(
                "O código do artista deve corresponder a um artista cadastrado.");
        }

        if (albumId is int id &&
            !await musicRepository.AlbumBelongsToArtistAsync(id, artistId, cancellationToken))
        {
            throw new InvalidMusicReferenceException(
                "O álbum informado deve pertencer ao artista selecionado.");
        }
    }

    private static MusicResponseDto Map(Music music)
    {
        return new MusicResponseDto(
            music.Id,
            music.Title,
            music.DurationSeconds,
            music.AudioUrl,
            music.CoverUrl,
            music.ArtistId,
            music.Artist?.Name,
            music.AlbumId,
            music.Album?.Title,
            music.Source.ToString(),
            music.YouTubeVideoId,
            music.ChannelTitle);
    }
}
