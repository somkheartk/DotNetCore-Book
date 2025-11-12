# บทที่ 7: Web API ด้วย ASP.NET Core

## 7.1 RESTful API คืออะไร

REST (Representational State Transfer) เป็น architectural style สำหรับ Web Services

### HTTP Methods:
- **GET**: ดึงข้อมูล
- **POST**: สร้างข้อมูลใหม่
- **PUT**: แก้ไขข้อมูลทั้งหมด
- **PATCH**: แก้ไขข้อมูลบางส่วน
- **DELETE**: ลบข้อมูล

### HTTP Status Codes:
- **200 OK**: สำเร็จ
- **201 Created**: สร้างสำเร็จ
- **204 No Content**: สำเร็จแต่ไม่มีข้อมูลส่งกลับ
- **400 Bad Request**: Request ไม่ถูกต้อง
- **404 Not Found**: ไม่พบข้อมูล
- **500 Internal Server Error**: Server error

## 7.2 สร้าง Web API Project

```bash
# สร้าง Web API project
dotnet new webapi -n MyWebApi
cd MyWebApi

# รันโปรเจค
dotnet run

# ทดสอบที่ https://localhost:5001/swagger
```

## 7.3 สร้าง API Controller

### 7.3.1 Simple API Controller

สร้างไฟล์ `Controllers/ProductsController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private static List<Product> _products = new()
        {
            new Product { Id = 1, Name = "โน้ตบุ๊ค", Price = 25000, Stock = 10 },
            new Product { Id = 2, Name = "เมาส์", Price = 590, Stock = 50 },
            new Product { Id = 3, Name = "คีย์บอร์ด", Price = 1290, Stock = 30 }
        };

        // GET: api/products
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return Ok(_products);
        }

        // GET: api/products/1
        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            
            if (product == null)
            {
                return NotFound(new { message = $"ไม่พบสินค้า ID: {id}" });
            }

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public ActionResult<Product> CreateProduct(Product product)
        {
            product.Id = _products.Max(p => p.Id) + 1;
            _products.Add(product);

            return CreatedAtAction(
                nameof(GetProduct), 
                new { id = product.Id }, 
                product
            );
        }

        // PUT: api/products/1
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest(new { message = "ID ไม่ตรงกัน" });
            }

            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound(new { message = $"ไม่พบสินค้า ID: {id}" });
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;

            return NoContent();
        }

        // DELETE: api/products/1
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { message = $"ไม่พบสินค้า ID: {id}" });
            }

            _products.Remove(product);
            return NoContent();
        }

        // GET: api/products/search?keyword=โน้ต
        [HttpGet("search")]
        public ActionResult<IEnumerable<Product>> SearchProducts([FromQuery] string keyword)
        {
            var results = _products
                .Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(results);
        }
    }
}
```

### 7.3.2 Product Model

สร้างไฟล์ `Models/Product.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "กรุณาระบุชื่อสินค้า")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "ราคาต้องมากกว่า 0")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "จำนวนต้องมากกว่า 0")]
        public int Stock { get; set; }
    }
}
```

## 7.4 ทดสอบ API

### 7.4.1 ใช้ Swagger/OpenAPI

Swagger UI จะเปิดอัตโนมัติที่ https://localhost:5001/swagger

### 7.4.2 ใช้ cURL

```bash
# GET all products
curl -X GET https://localhost:5001/api/products

# GET product by ID
curl -X GET https://localhost:5001/api/products/1

# POST new product
curl -X POST https://localhost:5001/api/products \
  -H "Content-Type: application/json" \
  -d '{"name":"หูฟัง","price":1990,"stock":20}'

# PUT update product
curl -X PUT https://localhost:5001/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{"id":1,"name":"โน้ตบุ๊ค Updated","price":26000,"stock":15}'

# DELETE product
curl -X DELETE https://localhost:5001/api/products/1
```

### 7.4.3 ใช้ C# HttpClient

```csharp
using System.Net.Http.Json;

var client = new HttpClient { BaseAddress = new Uri("https://localhost:5001") };

// GET
var products = await client.GetFromJsonAsync<List<Product>>("api/products");

// GET by ID
var product = await client.GetFromJsonAsync<Product>("api/products/1");

// POST
var newProduct = new Product { Name = "หูฟัง", Price = 1990, Stock = 20 };
var response = await client.PostAsJsonAsync("api/products", newProduct);

// PUT
product.Price = 2100;
await client.PutAsJsonAsync($"api/products/{product.Id}", product);

// DELETE
await client.DeleteAsync($"api/products/{product.Id}");
```

## 7.5 Data Transfer Objects (DTOs)

### 7.5.1 ทำไมต้องใช้ DTOs

- แยก internal model กับ API response
- ควบคุมข้อมูลที่ส่งออก
- Validation แยกจาก business model

### 7.5.2 สร้าง DTOs

```csharp
// DTOs/ProductDto.cs
namespace MyWebApi.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsAvailable => Stock > 0;
    }

    public class CreateProductDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }

    public class UpdateProductDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}
```

### 7.5.3 Controller with DTOs

```csharp
[HttpGet]
public ActionResult<IEnumerable<ProductDto>> GetProducts()
{
    var productDtos = _products.Select(p => new ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        Stock = p.Stock
    });

    return Ok(productDtos);
}

[HttpPost]
public ActionResult<ProductDto> CreateProduct(CreateProductDto dto)
{
    var product = new Product
    {
        Id = _products.Max(p => p.Id) + 1,
        Name = dto.Name,
        Price = dto.Price,
        Stock = dto.Stock
    };

    _products.Add(product);

    var productDto = new ProductDto
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Stock = product.Stock
    };

    return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
}
```

## 7.6 Error Handling

### 7.6.1 Global Exception Handler

สร้างไฟล์ `Middleware/ExceptionMiddleware.cs`:

```csharp
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "เกิดข้อผิดพลาด");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            statusCode = context.Response.StatusCode,
            message = "เกิดข้อผิดพลาดภายในระบบ",
            detailed = exception.Message
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}

// ใน Program.cs
app.UseMiddleware<ExceptionMiddleware>();
```

## 7.7 Authentication และ Authorization

### 7.7.1 JWT Authentication

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

app.UseAuthentication();
app.UseAuthorization();
```

### 7.7.2 Protected Endpoints

```csharp
[Authorize]
[HttpPost]
public ActionResult<Product> CreateProduct(Product product)
{
    // เฉพาะ authenticated users
}

[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public IActionResult DeleteProduct(int id)
{
    // เฉพาะ Admin
}
```

## 7.8 API Versioning

```bash
dotnet add package Microsoft.AspNetCore.Mvc.Versioning
```

```csharp
// Program.cs
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Controller
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetProducts()
    {
        // Version 1.0
    }
}

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsV2Controller : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<ProductV2>> GetProducts()
    {
        // Version 2.0
    }
}
```

## 7.9 CORS Configuration

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

    options.AddPolicy("AllowSpecific",
        builder => builder
            .WithOrigins("https://myapp.com")
            .WithMethods("GET", "POST")
            .WithHeaders("Content-Type"));
});

app.UseCors("AllowAll");
```

## 7.10 Rate Limiting

```bash
dotnet add package AspNetCoreRateLimit
```

```csharp
// Program.cs
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// appsettings.json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      }
    ]
  }
}
```

## 7.11 สรุป

ในบทนี้เราได้เรียนรู้:

✅ RESTful API คืออะไร
✅ การสร้าง Web API ด้วย ASP.NET Core
✅ HTTP Methods และ Status Codes
✅ การใช้ DTOs
✅ Error Handling
✅ Authentication และ Authorization
✅ API Versioning
✅ CORS และ Rate Limiting

## 📚 แบบฝึกหัด

1. สร้าง API สำหรับระบบจัดการหนังสือ
2. เพิ่ม Pagination ใน GET endpoints
3. Implement JWT Authentication
4. สร้าง API Documentation ด้วย Swagger
5. เพิ่ม Caching สำหรับ GET requests

---

**ก่อนหน้า:** [บทที่ 6 - การทำงานกับฐานข้อมูล](../Chapter06-Database/README.md)
**ต่อไป:** [บทที่ 8 - Dependency Injection](../Chapter08-DependencyInjection/README.md)
