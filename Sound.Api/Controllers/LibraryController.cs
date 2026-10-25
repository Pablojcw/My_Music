using Microsoft.AspNetCore.Mvc;
using Sound.Application.DTOs;
using Sound.Application.Services;

namespace Sound.Api.Controllers;

[ApiController]
[Route("api/biblioteca")]
public sealed class LibraryController(ILibraryService libraryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Lista as músicas da biblioteca")]
    [EndpointDescription("Retorna as músicas salvas, da mais recente para a mais antiga.")]
    [ProducesResponseType<IReadOnlyList<LibraryMusicResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LibraryMusicResponseDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var items = await libraryService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{musicId:int}")]
    [EndpointSummary("Busca uma música salva na biblioteca")]
    [EndpointDescription("Retorna a entrada da biblioteca correspondente ao código da música.")]
    [ProducesResponseType<LibraryMusicResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibraryMusicResponseDto>> GetByMusicId(
        int musicId,
        CancellationToken cancellationToken)
    {
        var item = await libraryService.GetByMusicIdAsync(musicId, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [EndpointSummary("Salva uma música na biblioteca")]
    [EndpointDescription("Adiciona uma música existente à biblioteca.")]
    [ProducesResponseType<LibraryMusicResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LibraryMusicResponseDto>> Add(
        AddLibraryMusicRequest request,
        CancellationToken cancellationToken)
    {
        var item = await libraryService.AddAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByMusicId), new { musicId = item.MusicId }, item);
    }

    [HttpDelete("{musicId:int}")]
    [EndpointSummary("Remove uma música da biblioteca")]
    [EndpointDescription("Remove a música salva pelo código, sem excluir a música do catálogo.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int musicId,
        CancellationToken cancellationToken)
    {
        var deleted = await libraryService.DeleteAsync(musicId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
