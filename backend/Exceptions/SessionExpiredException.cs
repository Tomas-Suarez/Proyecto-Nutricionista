namespace backend.Exceptions;

public class SessionExpiredException : Exception
{
    public SessionExpiredException()
            : base("Su sesión ha expirado. Por favor, ingrese nuevamente.") { }

    public SessionExpiredException(string message)
        : base(message) { }
}
