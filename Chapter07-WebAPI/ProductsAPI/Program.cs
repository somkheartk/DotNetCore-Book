using FluentValidation;
using FluentValidation.AspNetCore;
using ProductsAPI.Interfaces;
using ProductsAPI.Middleware;
using ProductsAPI.Repositories;
using ProductsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// เพิ่ม Controllers และ configure API behavior
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Custom validation error response
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors.Select(er => er.ErrorMessage).ToArray()
                );

            var errorResponse = new ProductsAPI.Models.ApiErrorResponse(
                statusCode: 400,
                message: "ข้อมูลที่ส่งมาไม่ถูกต้อง",
                errors: errors
            );

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(errorResponse);
        };
    });

// เพิ่ม FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ลงทะเบียน Dependency Injection
// Repository Pattern
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
// Service Layer
builder.Services.AddScoped<IProductService, ProductService>();

// เพิ่ม OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// เพิ่ม CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// ใช้ Custom Exception Handler Middleware (ต้องอยู่ก่อน middleware อื่นๆ)
app.UseCustomExceptionHandler();

// Swagger สำหรับทดสอบ API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API v1");
        options.RoutePrefix = string.Empty; // เปิด Swagger UI ที่ root URL
    });
}

app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
