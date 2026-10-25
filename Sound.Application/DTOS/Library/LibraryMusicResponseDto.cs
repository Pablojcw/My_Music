using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Informações de uma música salva na biblioteca.</summary>
public sealed record LibraryMusicResponseDto(
    [property: JsonPropertyName("id")]
    int Id,
    [property: JsonPropertyName("musicaId")]
    int MusicId,
    [property: JsonPropertyName("salvaEm")]
    DateTimeOffset SavedAt,
    [property: JsonPropertyName("musica")]
    MusicResponseDto Music);
