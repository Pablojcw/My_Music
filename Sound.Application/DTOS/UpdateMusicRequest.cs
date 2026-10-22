using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Dados para substituir as informações de uma música.</summary>
public sealed record UpdateMusicRequest
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres.")]
    [JsonPropertyName("titulo")]
    public required string Title { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "A duração deve ser maior que zero.")]
    [JsonPropertyName("duracaoSegundos")]
    public int DurationSeconds { get; init; }

    [JsonPropertyName("urlAudio")]
    public string? AudioUrl { get; init; }

    [JsonPropertyName("urlCapa")]
    public string? CoverUrl { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe o código de um artista válido.")]
    [JsonPropertyName("artistaId")]
    public int ArtistId { get; init; }

    [JsonPropertyName("albumId")]
    public int? AlbumId { get; init; }
}
