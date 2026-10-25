using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Dados para salvar uma música na biblioteca.</summary>
public sealed record AddLibraryMusicRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe o código de uma música válida.")]
    [JsonPropertyName("musicaId")]
    public int MusicId { get; init; }
}
