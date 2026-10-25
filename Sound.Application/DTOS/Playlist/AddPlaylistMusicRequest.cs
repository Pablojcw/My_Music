using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Dados para adicionar uma música a uma playlist.</summary>
public sealed record AddPlaylistMusicRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe o código de uma música válida.")]
    [JsonPropertyName("musicaId")]
    public int MusicId { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "A posição deve ser zero ou maior.")]
    [JsonPropertyName("posicao")]
    public int? Position { get; init; }
}
