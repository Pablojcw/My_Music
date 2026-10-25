using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Dados para substituir as informações de uma playlist.</summary>
public sealed record UpdatePlaylistRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres.")]
    [JsonPropertyName("nome")]
    public required string Name { get; init; }

    [JsonPropertyName("descricao")]
    public string? Description { get; init; }

    [JsonPropertyName("urlCapa")]
    public string? CoverUrl { get; init; }
}
