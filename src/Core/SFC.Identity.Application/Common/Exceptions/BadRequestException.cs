namespace SFC.Identity.Application.Common.Exceptions
{
    public class BadRequestException : Exception
    {
        public Dictionary<string, IEnumerable<string>> Errors { get; }

        public BadRequestException(string message, Dictionary<string, IEnumerable<string>> errors) : base(message)
        {
            Errors = errors;
        }

        public BadRequestException(string message, (string, string) singleError) : base(message)
        {
            Errors = new Dictionary<string, IEnumerable<string>> { { singleError.Item1, new List<string> { singleError.Item2 } } };
        }
    }
}
