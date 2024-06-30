namespace SFC.Identity.Application.Common.Exceptions;

public class IdentityException(string message, Dictionary<string, IEnumerable<string>> errors) : Exception(message)
{
    public Dictionary<string, IEnumerable<string>> Errors { get; } = errors;
}
