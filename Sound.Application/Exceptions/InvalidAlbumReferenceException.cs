namespace Sound.Application.Exceptions;

public sealed class InvalidAlbumReferenceException(string message) : Exception(message)
{
}
