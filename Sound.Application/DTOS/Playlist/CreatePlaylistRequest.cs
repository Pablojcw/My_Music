using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Dados necessários para criar uma playlist.</summary>
public sealed record CreatePlaylistRequest
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
