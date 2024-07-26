namespace Talabat.APIs.Errors
{
    public class ValidationErrorResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; set; }

        public ValidationErrorResponse()
            : base(400)
        {
            Errors = new List<string>();
        }
    }
}
