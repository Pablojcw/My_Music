using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Informações de uma música retornadas pela API.</summary>
public sealed record MusicResponseDto(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("titulo")]
    string Title,
    [property: JsonPropertyName("duracaoSegundos")]
    int DurationSeconds,
    [property: JsonPropertyName("urlAudio")]
    string? AudioUrl,
    [property: JsonPropertyName("urlCapa")]
    string? CoverUrl,
    [property: JsonPropertyName("artistaId")]
    int ArtistId,
    [property: JsonPropertyName("nomeArtista")]
    string ArtistName,
    [property: JsonPropertyName("albumId")]
    int? AlbumId,
    [property: JsonPropertyName("tituloAlbum")]
    string? AlbumTitle);
