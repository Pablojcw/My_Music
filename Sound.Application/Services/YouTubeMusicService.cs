using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sound.Application.DTOs;
using Sound.Application.Exceptions;
using Sound.Application.Interfaces;
using Sound.Domain.Entities;

namespace Sound.Application.Services;

public sealed class YouTubeMusicService(
    HttpClient httpClient,
    string? apiKey,
    IMusicRepository musicRepository) : IYouTubeMusicService
{
    private const string YouTubeVideoIdPattern = "^[A-Za-z0-9_-]{11}$";
    private const string ApiUnavailableMessage =
        "A pesquisa no YouTube está indisponível no momento. Tente novamente mais tarde.";

    public async Task<IReadOnlyList<YouTubeSearchResultDto>> SearchAsync(
        string query,
        int maxResults,
        CancellationToken cancellationToken)
    {
        var normalizedQuery = query.Trim();
        if (normalizedQuery.Length is 0 or > 100)
        {
            throw new ArgumentException("Informe uma busca entre 1 e 100 caracteres.");
        }

        if (maxResults is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxResults),
                "A quantidade de resultados deve estar entre 1 e 20.");
        }

        var apiKey = GetApiKey();
        var path = BuildPath(
            "search",
            ("part", "snippet"),
            ("type", "video"),
            ("q", normalizedQuery),
            ("maxResults", maxResults.ToString(System.Globalization.CultureInfo.InvariantCulture)),
            ("videoEmbeddable", "true"),
            ("regionCode", "BR"),
            ("relevanceLanguage", "pt"),
            ("key", apiKey));

        var response = await GetResponseAsync<YouTubeSearchResponse>(path, cancellationToken);
        return response.Items?
            .Where(item =>
                IsValidVideoId(item.Id?.VideoId) &&
                !string.IsNullOrWhiteSpace(item.Snippet?.Title))
            .Select(item =>
            {
                var videoId = item.Id!.VideoId!;
                return new YouTubeSearchResultDto(
                    videoId,
                    item.Snippet!.Title!,
                    string.IsNullOrWhiteSpace(item.Snippet.ChannelTitle)
                        ? "Canal não informado"
                        : item.Snippet.ChannelTitle,
                    SelectThumbnail(item.Snippet.Thumbnails),
                    $"https://www.youtube.com/embed/{videoId}");
            })
            .ToArray() ?? [];
    }

    public async Task<LibraryMusicResponseDto> SaveToLibraryAsync(
        SaveYouTubeVideoRequest request,
        CancellationToken cancellationToken)
    {
        if (!IsValidVideoId(request.VideoId))
        {
            throw new ArgumentException("O identificador do vídeo é inválido.");
        }

        var path = BuildPath(
            "videos",
            ("part", "snippet"),
            ("id", request.VideoId),
            ("key", GetApiKey()));
        var response = await GetResponseAsync<YouTubeVideoResponse>(path, cancellationToken);
        var video = response.Items?.FirstOrDefault(item => item.Id == request.VideoId);
        if (video?.Snippet is null || string.IsNullOrWhiteSpace(video.Snippet.Title))
        {
            throw new ResourceNotFoundException("O vídeo não foi encontrado ou não está disponível.");
        }

        var music = new Music
        {
            Title = video.Snippet.Title,
            DurationSeconds = null,
            AudioUrl = null,
            CoverUrl = SelectThumbnail(video.Snippet.Thumbnails),
            ArtistId = null,
            AlbumId = null,
            Source = MusicSource.YouTube,
            YouTubeVideoId = request.VideoId,
            ChannelTitle = video.Snippet.ChannelTitle
        };

        return Map(await musicRepository.SaveYouTubeVideoToLibraryAsync(
            music,
            cancellationToken));
    }

    private string GetApiKey()
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new YouTubeApiUnavailableException(
                "A integração com o YouTube não está configurada. Defina YouTube__ApiKey.");
        }

        return apiKey.Trim();
    }

    private async Task<T> GetResponseAsync<T>(
        string path,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync(
                path,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new YouTubeApiUnavailableException(ApiUnavailableMessage);
            }

            return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
                ?? throw new YouTubeApiUnavailableException(ApiUnavailableMessage);
        }
        catch (HttpRequestException)
        {
            throw new YouTubeApiUnavailableException(ApiUnavailableMessage);
        }
        catch (JsonException)
        {
            throw new YouTubeApiUnavailableException(ApiUnavailableMessage);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new YouTubeApiUnavailableException(ApiUnavailableMessage);
        }
    }

    private static string BuildPath(string resource, params (string Name, string Value)[] parameters)
    {
        return $"{resource}?{string.Join("&", parameters.Select(parameter =>
            $"{Uri.EscapeDataString(parameter.Name)}={Uri.EscapeDataString(parameter.Value)}"))}";
    }

    private static bool IsValidVideoId(string? videoId)
    {
        return videoId is not null &&
            System.Text.RegularExpressions.Regex.IsMatch(
                videoId,
                YouTubeVideoIdPattern,
                System.Text.RegularExpressions.RegexOptions.CultureInvariant);
    }

    private static string? SelectThumbnail(YouTubeThumbnails? thumbnails)
    {
        return thumbnails?.High?.Url
            ?? thumbnails?.Medium?.Url
            ?? thumbnails?.Default?.Url;
    }

    private static LibraryMusicResponseDto Map(LibraryMusic item)
    {
        var music = item.Music;
        var response = new MusicResponseDto(
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
            response);
    }

    private sealed class YouTubeSearchResponse
    {
        [JsonPropertyName("items")]
        public List<YouTubeSearchItem>? Items { get; init; }
    }

    private sealed class YouTubeSearchItem
    {
        [JsonPropertyName("id")]
        public YouTubeSearchId? Id { get; init; }

        [JsonPropertyName("snippet")]
        public YouTubeSnippet? Snippet { get; init; }
    }

    private sealed class YouTubeSearchId
    {
        [JsonPropertyName("videoId")]
        public string? VideoId { get; init; }
    }

    private sealed class YouTubeVideoResponse
    {
        [JsonPropertyName("items")]
        public List<YouTubeVideoItem>? Items { get; init; }
    }

    private sealed class YouTubeVideoItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; init; }

        [JsonPropertyName("snippet")]
        public YouTubeSnippet? Snippet { get; init; }
    }

    private sealed class YouTubeSnippet
    {
        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("channelTitle")]
        public string? ChannelTitle { get; init; }

        [JsonPropertyName("thumbnails")]
        public YouTubeThumbnails? Thumbnails { get; init; }
    }

    private sealed class YouTubeThumbnails
    {
        [JsonPropertyName("default")]
        public YouTubeThumbnail? Default { get; init; }

        [JsonPropertyName("medium")]
        public YouTubeThumbnail? Medium { get; init; }

        [JsonPropertyName("high")]
        public YouTubeThumbnail? High { get; init; }
    }

    private sealed class YouTubeThumbnail
    {
        [JsonPropertyName("url")]
        public string? Url { get; init; }
    }
}
