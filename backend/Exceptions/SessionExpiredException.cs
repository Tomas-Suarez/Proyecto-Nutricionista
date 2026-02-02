namespace backend.Exceptions;

public class SessionExpiredException : Exception
{
    public SessionExpiredException(string message = "Su sesión ha expirado. Por favor, ingrese nuevamente.")
        : base(message) { }
}
