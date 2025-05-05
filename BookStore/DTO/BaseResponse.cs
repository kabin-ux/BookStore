namespace BookStore.DTO
{
    public class BaseResponse<T>
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public BaseResponse(int code, bool success, string message, T? data = default)
        {
            StatusCode = code;
            Success = success;
            Message = message;
            Data = data;
        }
    }
}