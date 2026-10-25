using Sound.Application.DTOs;
using Sound.Application.Exceptions;
using Sound.Application.Interfaces;
using Sound.Domain.Entities;

namespace Sound.Application.Services;

public sealed class PlaylistService(IPlaylistRepository playlistRepository) : IPlaylistService
{
    public async Task<IReadOnlyList<PlaylistResponseDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var playlists = await playlistRepository.GetAllAsync(cancellationToken);
        return playlists.Select(Map).ToArray();
    }

    public async Task<PlaylistResponseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var playlist = await playlistRepository.GetByIdAsync(id, cancellationToken);
        return playlist is null ? null : Map(playlist);
    }

    public async Task<PlaylistResponseDto> CreateAsync(
        CreatePlaylistRequest request,
        CancellationToken cancellationToken)
    {
        var playlist = new Playlist
        {
            Name = request.Name,
            Description = request.Description,
            CoverUrl = request.CoverUrl,
            CreatAt = DateTime.UtcNow
        };

        return Map(await playlistRepository.AddAsync(playlist, cancellationToken));
    }

    public async Task<PlaylistResponseDto?> UpdateAsync(
        int id,
        UpdatePlaylistRequest request,
        CancellationToken cancellationToken)
    {
        var updatedPlaylist = new Playlist
        {
            Name = request.Name,
            Description = request.Description,
            CoverUrl = request.CoverUrl
        };

        var playlist = await playlistRepository.UpdateAsync(id, updatedPlaylist, cancellationToken);
        return playlist is null ? null : Map(playlist);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return playlistRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<PlaylistMusicResponseDto>> GetMusicsAsync(
        int playlistId,
        CancellationToken cancellationToken)
    {
        if (await playlistRepository.GetMusicCountAsync(playlistId, cancellationToken) is null)
        {
            throw new ResourceNotFoundException("A playlist informada não foi encontrada.");
        }

        var items = await playlistRepository.GetMusicsAsync(playlistId, cancellationToken);
        return items.Select(Map).ToArray();
    }

    public async Task<PlaylistMusicResponseDto> AddMusicAsync(
        int playlistId,
        AddPlaylistMusicRequest request,
        CancellationToken cancellationToken)
    {
        var count = await playlistRepository.GetMusicCountAsync(playlistId, cancellationToken);
        if (count is null)
        {
            throw new ResourceNotFoundException("A playlist informada não foi encontrada.");
        }

        if (!await playlistRepository.MusicExistsAsync(request.MusicId, cancellationToken))
        {
            throw new ResourceNotFoundException("A música informada não foi encontrada.");
        }

        if (await playlistRepository.ContainsMusicAsync(
                playlistId,
                request.MusicId,
                cancellationToken))
        {
            throw new ResourceConflictException("A música já está nesta playlist.");
        }

        var position = request.Position ?? count.Value;
        if (position > count.Value)
        {
            throw new InvalidPlaylistPositionException(
                "A posição deve estar entre zero e a quantidade atual de músicas da playlist.");
        }

        var item = await playlistRepository.AddMusicAsync(
            playlistId,
            request.MusicId,
            position,
            cancellationToken);
        return Map(item);
    }

    public async Task<PlaylistMusicResponseDto?> UpdateMusicPositionAsync(
        int playlistId,
        int musicId,
        UpdatePlaylistMusicPositionRequest request,
        CancellationToken cancellationToken)
    {
        var count = await playlistRepository.GetMusicCountAsync(playlistId, cancellationToken);
        if (count is null)
        {
            throw new ResourceNotFoundException("A playlist informada não foi encontrada.");
        }

        if (!await playlistRepository.ContainsMusicAsync(playlistId, musicId, cancellationToken))
        {
            return null;
        }

        if (request.Position >= count.Value)
        {
            throw new InvalidPlaylistPositionException(
                "A posição deve estar entre zero e a quantidade de músicas menos um.");
        }

        var item = await playlistRepository.UpdateMusicPositionAsync(
            playlistId,
            musicId,
            request.Position,
            cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<bool> RemoveMusicAsync(
        int playlistId,
        int musicId,
        CancellationToken cancellationToken)
    {
        if (await playlistRepository.GetMusicCountAsync(playlistId, cancellationToken) is null)
        {
            throw new ResourceNotFoundException("A playlist informada não foi encontrada.");
        }

        return await playlistRepository.RemoveMusicAsync(
            playlistId,
            musicId,
            cancellationToken);
    }

    private static PlaylistResponseDto Map(Playlist playlist)
    {
        return new PlaylistResponseDto(
            playlist.Id,
            playlist.Name,
            playlist.Description,
            playlist.CoverUrl,
            ToDateTimeOffset(playlist.CreatAt));
    }

    private static PlaylistMusicResponseDto Map(PlaylistMusic item)
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

        return new PlaylistMusicResponseDto(
            item.PlaylistId,
            item.MusicId,
            item.Position,
            ToDateTimeOffset(item.AddedAt),
            musicResponse);
    }

    private static DateTimeOffset ToDateTimeOffset(DateTime value)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }
}
