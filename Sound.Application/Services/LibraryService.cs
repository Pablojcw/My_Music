using Sound.Application.DTOs;
using Sound.Application.Exceptions;
using Sound.Application.Interfaces;
using Sound.Domain.Entities;

namespace Sound.Application.Services;

public sealed class LibraryService(ILibraryRepository libraryRepository) : ILibraryService
{
    public async Task<IReadOnlyList<LibraryMusicResponseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var items = await libraryRepository.GetAllAsync(cancellationToken);
        return items.Select(Map).ToArray();
    }

    public async Task<LibraryMusicResponseDto?> GetByMusicIdAsync(
        int musicId,
        CancellationToken cancellationToken)
    {
        var item = await libraryRepository.GetByMusicIdAsync(musicId, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<LibraryMusicResponseDto> AddAsync(
        AddLibraryMusicRequest request,
        CancellationToken cancellationToken)
    {
        if (!await libraryRepository.MusicExistsAsync(request.MusicId, cancellationToken))
        {
            throw new ResourceNotFoundException("A música informada não foi encontrada.");
        }

        if (await libraryRepository.ContainsMusicAsync(request.MusicId, cancellationToken))
        {
            throw new ResourceConflictException("A música já está salva na biblioteca.");
        }

        return Map(await libraryRepository.AddAsync(request.MusicId, cancellationToken));
    }

    public Task<bool> DeleteAsync(int musicId, CancellationToken cancellationToken)
    {
        return libraryRepository.DeleteAsync(musicId, cancellationToken);
    }

    private static LibraryMusicResponseDto Map(LibraryMusic item)
    {
        var music = item.Music;
        var musicResponse = new MusicResponseDto(
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

        return new LibraryMusicResponseDto(
            item.Id,
            item.MusicId,
            new DateTimeOffset(DateTime.SpecifyKind(item.SavedAt, DateTimeKind.Utc)),
            musicResponse);
    }
}
