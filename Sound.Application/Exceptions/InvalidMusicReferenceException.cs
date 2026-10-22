namespace Sound.Application.Exceptions;

public sealed class InvalidMusicReferenceException(string message) : Exception(message)
{
}
