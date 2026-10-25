using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sound.Application.Exceptions;

namespace Sound.Api.Middleware;

public sealed class YouTubeApiExceptionHandler(
    ILogger<YouTubeApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not YouTubeApiUnavailableException)
        {
            return false;
        }

        logger.LogWarning(
            "A integração com o YouTube está indisponível. Motivo: {FailureType}",
            exception.GetType().Name);
        httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status503ServiceUnavailable,
            Title = "Pesquisa indisponível",
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        }, cancellationToken);

        return true;
    }
}
