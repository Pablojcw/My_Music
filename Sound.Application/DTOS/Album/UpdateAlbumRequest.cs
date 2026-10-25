using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Dados para substituir as informações de um álbum.</summary>
public sealed record UpdateAlbumRequest
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres.")]
    [JsonPropertyName("titulo")]
    public required string Title { get; init; }

    [JsonPropertyName("dataLancamento")]
    public required DateTimeOffset ReleaseDate { get; init; }

    [JsonPropertyName("urlCapa")]
    public string? CoverUrl { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Informe o código de um artista válido.")]
    [JsonPropertyName("artistaId")]
    public int ArtistId { get; init; }
}
