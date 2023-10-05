namespace SFC.Identity.Application.Common.Exceptions;

public class JwtException : Exception
{
    public JwtException(string message) : base(message) { }
}
