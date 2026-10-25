using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Informações de uma playlist retornadas pela API.</summary>
public sealed record PlaylistResponseDto(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("nome")]
    string Name,
    [property: JsonPropertyName("descricao")]
    string? Description,
    [property: JsonPropertyName("urlCapa")]
    string? CoverUrl,
    [property: JsonPropertyName("criadaEm")]
    DateTimeOffset CreatedAt);
