using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sound.Application.Exceptions;

namespace Sound.Api.Middleware;

public sealed class InvalidAlbumReferenceExceptionHandler(
    ILogger<InvalidAlbumReferenceExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not InvalidAlbumReferenceException invalidReferenceException)
        {
            return false;
        }

        logger.LogWarning(exception, "Referência de álbum inválida na solicitação.");
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Referência de álbum inválida",
            Detail = invalidReferenceException.Message,
            Instance = httpContext.Request.Path
        }, cancellationToken);

        return true;
    }
}
