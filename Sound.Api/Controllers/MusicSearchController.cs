using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Sound.Application.DTOs;
using Sound.Application.Services;

namespace Sound.Api.Controllers;

[ApiController]
[Route("api/music-search")]
public sealed class MusicSearchController(IYouTubeMusicService youtubeMusicService) : ControllerBase
{
    [HttpGet]
    [EnableRateLimiting("youtube-search")]
    [EndpointSummary("Pesquisa vídeos musicais no YouTube")]
    [EndpointDescription("Retorna resultados temporários do YouTube sem cadastrá-los no catálogo.")]
    [ProducesResponseType<IReadOnlyList<YouTubeSearchResultDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<IReadOnlyList<YouTubeSearchResultDto>>> Search(
        [FromQuery(Name = "q"), Required, StringLength(100, MinimumLength = 1)] string query,
        [FromQuery, Range(1, 20)] int maxResults = 10,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Busca inválida",
                Detail = "Informe o nome de uma música, artista ou vídeo.",
                Instance = Request.Path
            });
        }

        var results = await youtubeMusicService.SearchAsync(
            query,
            maxResults,
            cancellationToken);
        return Ok(results);
    }

    [HttpPost("biblioteca")]
    [EnableRateLimiting("youtube-search")]
    [EndpointSummary("Salva um vídeo do YouTube na biblioteca")]
    [EndpointDescription("Busca os dados do vídeo no YouTube e salva sua referência para uso na biblioteca e playlists.")]
    [ProducesResponseType<LibraryMusicResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<LibraryMusicResponseDto>> SaveToLibrary(
        SaveYouTubeVideoRequest request,
        CancellationToken cancellationToken)
    {
        var savedMusic = await youtubeMusicService.SaveToLibraryAsync(request, cancellationToken);
        return CreatedAtAction(
            nameof(LibraryController.GetByMusicId),
            "Library",
            new { musicId = savedMusic.MusicId },
            savedMusic);
    }
}
