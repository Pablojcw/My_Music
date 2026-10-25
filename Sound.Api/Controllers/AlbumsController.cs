using Microsoft.AspNetCore.Mvc;
using Sound.Application.DTOs;
using Sound.Application.Services;

namespace Sound.Api.Controllers;

[ApiController]
[Route("api/albuns")]
public sealed class AlbumsController(IAlbumService albumService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Lista os álbuns cadastrados")]
    [EndpointDescription("Retorna todos os álbuns cadastrados, incluindo o nome do artista.")]
    [ProducesResponseType<IReadOnlyList<AlbumResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AlbumResponseDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var albums = await albumService.GetAllAsync(cancellationToken);
        return Ok(albums);
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Busca um álbum pelo código")]
    [EndpointDescription("Retorna os dados do álbum correspondente ao código informado.")]
    [ProducesResponseType<AlbumResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlbumResponseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var album = await albumService.GetByIdAsync(id, cancellationToken);
        return album is null ? NotFound() : Ok(album);
    }

    [HttpPost]
    [EndpointSummary("Cadastra um álbum")]
    [EndpointDescription("Cadastra um álbum associado a um artista existente.")]
    [ProducesResponseType<AlbumResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlbumResponseDto>> Create(
        CreateAlbumRequest request,
        CancellationToken cancellationToken)
    {
        var album = await albumService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = album.Id }, album);
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Atualiza um álbum")]
    [EndpointDescription("Substitui as informações do álbum correspondente ao código informado.")]
    [ProducesResponseType<AlbumResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlbumResponseDto>> Update(
        int id,
        UpdateAlbumRequest request,
        CancellationToken cancellationToken)
    {
        var album = await albumService.UpdateAsync(id, request, cancellationToken);
        return album is null ? NotFound() : Ok(album);
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Exclui um álbum")]
    [EndpointDescription("Exclui o álbum e mantém as músicas cadastradas sem álbum.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await albumService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
