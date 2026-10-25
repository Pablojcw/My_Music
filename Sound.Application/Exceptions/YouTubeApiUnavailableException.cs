namespace Sound.Application.Exceptions;

public sealed class YouTubeApiUnavailableException(string message) : Exception(message)
{
}
