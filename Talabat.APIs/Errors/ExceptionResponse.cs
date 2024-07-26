namespace Talabat.APIs.Errors
{
    public class ExceptionResponse : ApiResponse
    {
        public string? Details { get; set; }
        public ExceptionResponse(int code, string? msg = null, string? details = null)
        : base(code, msg)
        {
            Details = details;
        }
    }
}
