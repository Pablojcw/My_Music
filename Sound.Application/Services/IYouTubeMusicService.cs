using Sound.Application.DTOs;

namespace Sound.Application.Services;

public interface IYouTubeMusicService
{
    Task<IReadOnlyList<YouTubeSearchResultDto>> SearchAsync(
        string query,
        int maxResults,
        CancellationToken cancellationToken);

    Task<LibraryMusicResponseDto> SaveToLibraryAsync(
        SaveYouTubeVideoRequest request,
        CancellationToken cancellationToken);
}
