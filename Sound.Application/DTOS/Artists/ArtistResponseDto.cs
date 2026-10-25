using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Informações de um artista retornadas pela API.</summary>
public sealed record ArtistResponseDto(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("nome")]
    string Name,
    [property: JsonPropertyName("biografia")]
    string? Biography,
    [property: JsonPropertyName("urlImagem")]
    string? ImageUrl);
