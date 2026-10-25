using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Informações de uma música adicionada a uma playlist.</summary>
public sealed record PlaylistMusicResponseDto(
    [property: JsonPropertyName("playlistId")]
    int PlaylistId,
    [property: JsonPropertyName("musicaId")]
    int MusicId,
    [property: JsonPropertyName("posicao")]
    int Position,
    [property: JsonPropertyName("adicionadaEm")]
    DateTimeOffset AddedAt,
    [property: JsonPropertyName("musica")]
    MusicResponseDto Music);
