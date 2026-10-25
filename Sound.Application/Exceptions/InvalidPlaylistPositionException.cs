namespace Sound.Application.Exceptions;

public sealed class InvalidPlaylistPositionException(string message) : Exception(message)
{
}
