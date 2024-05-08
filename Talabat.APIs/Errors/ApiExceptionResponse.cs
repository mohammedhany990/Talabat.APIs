namespace Talabat.APIs.Errors
{
    public class ApiExceptionResponse : ApiResponse
    {
        public string? Deatils { get; set; }
        public ApiExceptionResponse(int statusCode, string? message = null, string? details =null)
            : base(statusCode, message)
        {
            Deatils = details;
        }
    }
}
