namespace SFC.Identity.Application.Common.Exceptions;

public class IdentityException : Exception
{
    public Dictionary<string, IEnumerable<string>> Errors { get; }

    public IdentityException()
    {
        Errors = [];
    }

    public IdentityException(string message) : base(message)
    {
        Errors = [];
    }

    public IdentityException(string message, Exception innerException) : base(message, innerException)
    {
        Errors = [];
    }

    public IdentityException(string message, Dictionary<string, IEnumerable<string>> errors) : base(message)
    {
        Errors = errors;
    }
}