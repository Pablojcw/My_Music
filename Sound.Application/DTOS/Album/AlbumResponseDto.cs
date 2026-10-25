using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Informações de um álbum retornadas pela API.</summary>
public sealed record AlbumResponseDto(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("titulo")]
    string Title,
    [property: JsonPropertyName("dataLancamento")]
    DateTimeOffset ReleaseDate,
    [property: JsonPropertyName("urlCapa")]
    string? CoverUrl,
    [property: JsonPropertyName("artistaId")]
    int ArtistId,
    [property: JsonPropertyName("nomeArtista")]
    string ArtistName);