using System.Net;
using System.Text.Json;
using ProductsAPI.Models;

namespace ProductsAPI.Middleware
{
    /// <summary>
    /// Global Exception Handler Middleware
    /// จัดการ exception ทั้งหมดในแอปพลิเคชันและส่ง response ที่สม่ำเสมอ
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "เกิดข้อผิดพลาดที่ไม่คาดคิด: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new ApiErrorResponse(
                statusCode: context.Response.StatusCode,
                message: "เกิดข้อผิดพลาดภายในระบบ กรุณาลองใหม่อีกครั้งหรือติดต่อผู้ดูแลระบบ"
            );

            // แสดง details เฉพาะใน Development environment เท่านั้น
            if (_environment.IsDevelopment())
            {
                errorResponse.Details = exception.ToString();
            }

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    /// <summary>
    /// Extension method สำหรับลงทะเบียน middleware
    /// </summary>
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
