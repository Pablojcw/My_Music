using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Dados para substituir as informações de um artista.</summary>
public sealed record UpdateArtistRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres.")]
    [JsonPropertyName("nome")]
    public required string Name { get; init; }

    [JsonPropertyName("biografia")]
    public string? Biography { get; init; }

    [JsonPropertyName("urlImagem")]
    public string? ImageUrl { get; init; }
}
