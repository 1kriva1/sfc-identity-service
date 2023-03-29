namespace SFC.Identity.Application.Common.Exceptions
{
    public class IdentityException : Exception
    {
        public Dictionary<string, IEnumerable<string>> Errors { get; }

        public IdentityException(string message, Dictionary<string, IEnumerable<string>> errors) : base(message)
        {
            Errors = errors;
        }
    }
}
