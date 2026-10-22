using Microsoft.AspNetCore.Mvc;
using Sound.Application.DTOs;
using Sound.Application.Services;

namespace Sound.Api.Controllers;

[ApiController]
[Route("api/musicas")]
public sealed class MusicsController(IMusicService musicService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Lista as músicas cadastradas")]
    [EndpointDescription("Retorna todas as músicas cadastradas, incluindo os dados do artista e do álbum.")]
    [ProducesResponseType<IReadOnlyList<MusicResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MusicResponseDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var musics = await musicService.GetAllAsync(cancellationToken);
        return Ok(musics);
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Busca uma música pelo código")]
    [EndpointDescription("Retorna os dados da música correspondente ao código informado.")]
    [ProducesResponseType<MusicResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MusicResponseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var music = await musicService.GetByIdAsync(id, cancellationToken);
        return music is null ? NotFound() : Ok(music);
    }

    [HttpPost]
    [EndpointSummary("Cadastra uma música")]
    [EndpointDescription("Cadastra uma música para um artista existente. O álbum, quando informado, deve pertencer ao artista.")]
    [ProducesResponseType<MusicResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MusicResponseDto>> Create(
        CreateMusicRequest request,
        CancellationToken cancellationToken)
    {
        var music = await musicService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = music.Id }, music);
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Atualiza uma música")]
    [EndpointDescription("Substitui os dados da música correspondente ao código informado.")]
    [ProducesResponseType<MusicResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MusicResponseDto>> Update(
        int id,
        UpdateMusicRequest request,
        CancellationToken cancellationToken)
    {
        var music = await musicService.UpdateAsync(id, request, cancellationToken);
        return music is null ? NotFound() : Ok(music);
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Exclui uma música")]
    [EndpointDescription("Exclui a música correspondente ao código informado.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await musicService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
