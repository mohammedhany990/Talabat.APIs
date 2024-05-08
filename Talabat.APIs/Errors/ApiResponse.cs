namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetResponse(StatusCode);
        }

        private string? GetResponse(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "You are not Authorized",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }
}
