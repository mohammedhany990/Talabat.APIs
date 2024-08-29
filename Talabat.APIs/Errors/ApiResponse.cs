namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ApiResponse(int code, string? msg = null)
        {
            StatusCode = code;
            Message = msg ?? GetDefaultMsg(code);
        }

        private string? GetDefaultMsg(int code)
        {
            return code switch
            {
                400 => "Bad Request",
                401 => "UnAuthorized",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }
}
