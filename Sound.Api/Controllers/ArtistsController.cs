using Microsoft.AspNetCore.Mvc;
using Sound.Application.DTOs;
using Sound.Application.Services;

namespace Sound.Api.Controllers;

[ApiController]
[Route("api/artistas")]
public sealed class ArtistsController(IArtistService artistService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Lista os artistas cadastrados")]
    [EndpointDescription("Retorna todos os artistas cadastrados.")]
    [ProducesResponseType<IReadOnlyList<ArtistResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ArtistResponseDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var artists = await artistService.GetAllAsync(cancellationToken);
        return Ok(artists);
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Busca um artista pelo código")]
    [EndpointDescription("Retorna os dados do artista correspondente ao código informado.")]
    [ProducesResponseType<ArtistResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArtistResponseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var artist = await artistService.GetByIdAsync(id, cancellationToken);
        return artist is null ? NotFound() : Ok(artist);
    }

    [HttpPost]
    [EndpointSummary("Cadastra um artista")]
    [EndpointDescription("Cadastra um artista. O nome é obrigatório; a biografia e a URL da imagem são opcionais.")]
    [ProducesResponseType<ArtistResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ArtistResponseDto>> Create(
        CreateArtistRequest request,
        CancellationToken cancellationToken)
    {
        var artist = await artistService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = artist.Id }, artist);
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Atualiza um artista")]
    [EndpointDescription("Substitui as informações do artista correspondente ao código informado.")]
    [ProducesResponseType<ArtistResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArtistResponseDto>> Update(
        int id,
        UpdateArtistRequest request,
        CancellationToken cancellationToken)
    {
        var artist = await artistService.UpdateAsync(id, request, cancellationToken);
        return artist is null ? NotFound() : Ok(artist);
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Exclui um artista")]
    [EndpointDescription("Exclui o artista correspondente ao código informado.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await artistService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
