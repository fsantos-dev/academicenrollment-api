namespace MiApp.Domain.Exceptions;
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message = "Credenciales inválidas") 
        : base(message) { }
}