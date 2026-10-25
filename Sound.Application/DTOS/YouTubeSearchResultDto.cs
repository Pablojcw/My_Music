using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Resultado de busca de vídeo musical no YouTube.</summary>
public sealed record YouTubeSearchResultDto(
    [property: JsonPropertyName("videoId")]
    string VideoId,
    [property: JsonPropertyName("title")]
    string Title,
    [property: JsonPropertyName("channelTitle")]
    string ChannelTitle,
    [property: JsonPropertyName("thumbnailUrl")]
    string? ThumbnailUrl,
    [property: JsonPropertyName("embedUrl")]
    string EmbedUrl);
