using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sound.Application.DTOs;

/// <summary>Nova posição de uma música dentro da playlist, começando em zero.</summary>
public sealed record UpdatePlaylistMusicPositionRequest
{
    [Range(0, int.MaxValue, ErrorMessage = "A posição deve ser zero ou maior.")]
    [JsonPropertyName("posicao")]
    public int Position { get; init; }
}
