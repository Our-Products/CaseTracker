namespace CaseTrackerApplication.Exceptions
{
    public class ValidationException : AppException
    {
        public List<string> Errors { get; }

        public ValidationException(List<string> errors)
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new List<string> { message };
        }
    }
}