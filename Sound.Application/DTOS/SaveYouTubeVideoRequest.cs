using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Identificador do vídeo do YouTube a salvar na biblioteca.</summary>
public sealed record SaveYouTubeVideoRequest
{
    [Required]
    [RegularExpression("^[A-Za-z0-9_-]{11}$", ErrorMessage = "O identificador do vídeo é inválido.")]
    [JsonPropertyName("videoId")]
    public required string VideoId { get; init; }
}
