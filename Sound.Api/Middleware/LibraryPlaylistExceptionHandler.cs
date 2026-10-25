using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sound.Application.Exceptions;

namespace Sound.Api.Middleware;

public sealed class LibraryPlaylistExceptionHandler(
    ILogger<LibraryPlaylistExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ResourceNotFoundException notFound =>
                (StatusCodes.Status404NotFound, "Recurso não encontrado", notFound.Message),
            ResourceConflictException conflict =>
                (StatusCodes.Status409Conflict, "Conflito de recurso", conflict.Message),
            InvalidPlaylistPositionException invalidPosition =>
                (StatusCodes.Status400BadRequest, "Posição inválida", invalidPosition.Message),
            _ => (0, string.Empty, string.Empty)
        };

        if (statusCode == 0)
        {
            return false;
        }

        logger.LogWarning(exception, "Erro na operação de playlist ou biblioteca.");
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        }, cancellationToken);

        return true;
    }
}
