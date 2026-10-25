using Microsoft.AspNetCore.Mvc;
using Sound.Application.DTOs;
using Sound.Application.Services;

namespace Sound.Api.Controllers;

[ApiController]
[Route("api/playlists")]
public sealed class PlaylistsController(IPlaylistService playlistService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Lista as playlists cadastradas")]
    [EndpointDescription("Retorna todas as playlists cadastradas.")]
    [ProducesResponseType<IReadOnlyList<PlaylistResponseDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PlaylistResponseDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var playlists = await playlistService.GetAllAsync(cancellationToken);
        return Ok(playlists);
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Busca uma playlist pelo código")]
    [EndpointDescription("Retorna os dados da playlist correspondente ao código informado.")]
    [ProducesResponseType<PlaylistResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlaylistResponseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var playlist = await playlistService.GetByIdAsync(id, cancellationToken);
        return playlist is null ? NotFound() : Ok(playlist);
    }

    [HttpPost]
    [EndpointSummary("Cria uma playlist")]
    [EndpointDescription("Cria uma playlist com nome e informações opcionais de descrição e capa.")]
    [ProducesResponseType<PlaylistResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlaylistResponseDto>> Create(
        CreatePlaylistRequest request,
        CancellationToken cancellationToken)
    {
        var playlist = await playlistService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = playlist.Id }, playlist);
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Atualiza uma playlist")]
    [EndpointDescription("Substitui as informações da playlist correspondente ao código informado.")]
    [ProducesResponseType<PlaylistResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlaylistResponseDto>> Update(
        int id,
        UpdatePlaylistRequest request,
        CancellationToken cancellationToken)
    {
        var playlist = await playlistService.UpdateAsync(id, request, cancellationToken);
        return playlist is null ? NotFound() : Ok(playlist);
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Exclui uma playlist")]
    [EndpointDescription("Exclui a playlist e os vínculos com suas músicas, sem excluir as músicas.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await playlistService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id:int}/musicas")]
    [EndpointSummary("Lista as músicas de uma playlist")]
    [EndpointDescription("Retorna as músicas da playlist na ordem atual.")]
    [ProducesResponseType<IReadOnlyList<PlaylistMusicResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PlaylistMusicResponseDto>>> GetMusics(
        int id,
        CancellationToken cancellationToken)
    {
        var musics = await playlistService.GetMusicsAsync(id, cancellationToken);
        return Ok(musics);
    }

    [HttpPost("{id:int}/musicas")]
    [EndpointSummary("Adiciona uma música à playlist")]
    [EndpointDescription("Adiciona uma música ao final ou na posição zero-based informada.")]
    [ProducesResponseType<PlaylistMusicResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PlaylistMusicResponseDto>> AddMusic(
        int id,
        AddPlaylistMusicRequest request,
        CancellationToken cancellationToken)
    {
        var item = await playlistService.AddMusicAsync(id, request, cancellationToken);
        return CreatedAtAction(
            nameof(GetMusics),
            new { id },
            item);
    }

    [HttpPut("{playlistId:int}/musicas/{musicId:int}/posicao")]
    [EndpointSummary("Altera a posição de uma música na playlist")]
    [EndpointDescription("Move a música para a posição zero-based informada e atualiza a ordem das demais.")]
    [ProducesResponseType<PlaylistMusicResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlaylistMusicResponseDto>> UpdateMusicPosition(
        int playlistId,
        int musicId,
        UpdatePlaylistMusicPositionRequest request,
        CancellationToken cancellationToken)
    {
        var item = await playlistService.UpdateMusicPositionAsync(
            playlistId,
            musicId,
            request,
            cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{playlistId:int}/musicas/{musicId:int}")]
    [EndpointSummary("Remove uma música da playlist")]
    [EndpointDescription("Remove o vínculo da música com a playlist, sem excluir a música.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMusic(
        int playlistId,
        int musicId,
        CancellationToken cancellationToken)
    {
        var removed = await playlistService.RemoveMusicAsync(
            playlistId,
            musicId,
            cancellationToken);
        return removed ? NoContent() : NotFound();
    }
}
