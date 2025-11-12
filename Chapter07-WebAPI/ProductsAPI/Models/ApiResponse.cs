namespace ProductsAPI.Models
{
    /// <summary>
    /// Standard API Response wrapper สำหรับ response ที่สำเร็จ
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ApiResponse(T data, string message = "สำเร็จ")
        {
            Success = true;
            Message = message;
            Data = data;
        }

        public ApiResponse(string message)
        {
            Success = true;
            Message = message;
        }
    }

    /// <summary>
    /// Standard API Error Response สำหรับ response ที่เกิดข้อผิดพลาด
    /// </summary>
    public class ApiErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public string? Details { get; set; }

        public ApiErrorResponse(int statusCode, string message, Dictionary<string, string[]>? errors = null)
        {
            StatusCode = statusCode;
            Message = message;
            Errors = errors;
        }
    }
}
