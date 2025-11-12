# บทที่ 7: Web API ด้วย Best Practices - โปรเจคตัวอย่างแบบมืออาชีพ

## 📖 ภาพรวม

โปรเจคนี้เป็นตัวอย่าง Web API ที่สร้างด้วย ASP.NET Core พร้อม Best Practices ครบถ้วน เหมาะสำหรับนักพัฒนาที่ต้องการเรียนรู้การสร้าง API แบบมืออาชีพ

## 🏗️ โครงสร้างโปรเจค (Professional Structure)

```
ProductsAPI/
├── Controllers/           # API Controllers
│   └── ProductsController.cs
├── DTOs/                 # Data Transfer Objects
│   └── ProductDtos.cs
├── Interfaces/           # Interfaces สำหรับ Dependency Injection
│   ├── IProductRepository.cs
│   └── IProductService.cs
├── Middleware/           # Custom Middleware
│   └── ExceptionHandlerMiddleware.cs
├── Models/               # Domain Models/Entities
│   ├── ApiResponse.cs
│   └── Product.cs
├── Repositories/         # Repository Pattern Implementation
│   └── ProductRepository.cs
├── Services/             # Business Logic Layer
│   └── ProductService.cs
├── Validators/           # FluentValidation Validators
│   └── ProductValidators.cs
└── Program.cs           # Application Entry Point
```

## 🎯 Best Practices ที่ใช้ในโปรเจคนี้

### 1. **Layered Architecture (สถาปัตยกรรมแบบแยกชั้น)**

```
┌─────────────────────────────────┐
│     Controllers (API Layer)     │  <- รับ HTTP Requests และส่ง Responses
├─────────────────────────────────┤
│    Services (Business Logic)    │  <- จัดการ Business Logic
├─────────────────────────────────┤
│  Repositories (Data Access)     │  <- จัดการการเข้าถึงข้อมูล
├─────────────────────────────────┤
│     Models/Entities (Data)      │  <- โครงสร้างข้อมูล
└─────────────────────────────────┘
```

**ประโยชน์:**
- แยกความรับผิดชอบ (Separation of Concerns)
- ทดสอบง่าย (Testable)
- แก้ไขบำรุงรักษาง่าย (Maintainable)
- ขยายระบบได้ง่าย (Scalable)

### 2. **Repository Pattern**

**ไฟล์:** `Repositories/ProductRepository.cs`

ใช้ Repository Pattern เพื่อแยก data access logic ออกจาก business logic:

```csharp
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    // ...
}
```

**ประโยชน์:**
- เปลี่ยน data source ได้ง่าย (In-Memory → Database → API)
- ทดสอบได้ง่ายด้วย Mock
- Code reusability

### 3. **Service Layer Pattern**

**ไฟล์:** `Services/ProductService.cs`

แยก business logic ออกจาก controller:

```csharp
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    
    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
    {
        // Business logic here
        // Validation, transformation, etc.
    }
}
```

**ประโยชน์:**
- Controller บางลง (Thin Controller)
- Business logic ทดสอบได้อิสระ
- Reuse logic ได้หลาย controller

### 4. **DTOs (Data Transfer Objects)**

**ไฟล์:** `DTOs/ProductDtos.cs`

แยก DTO สำหรับ Request และ Response:

```csharp
// สำหรับการสร้าง (Request)
public class CreateProductDto { }

// สำหรับการแก้ไข (Request)
public class UpdateProductDto { }

// สำหรับการส่งกลับ (Response)
public class ProductResponseDto { }
```

**ประโยชน์:**
- ควบคุมข้อมูลที่ส่งเข้า/ออกได้
- ป้องกันการ over-posting
- แยก API contract จาก internal models
- เพิ่ม/ลด properties ได้ตามต้องการ

### 5. **FluentValidation**

**ไฟล์:** `Validators/ProductValidators.cs`

ใช้ FluentValidation แทน Data Annotations:

```csharp
public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("ชื่อสินค้าจำเป็นต้องระบุ")
            .MaximumLength(200).WithMessage("ชื่อสินค้าต้องไม่เกิน 200 ตัวอักษร");
            
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("ราคาต้องมากกว่า 0");
    }
}
```

**ประโยชน์:**
- Validation rules อยู่ในที่เดียว
- Complex validation ทำได้ง่าย
- Custom validation rules
- Error messages ชัดเจน
- ทดสอบได้อิสระ

### 6. **Global Exception Handling**

**ไฟล์:** `Middleware/ExceptionHandlerMiddleware.cs`

Middleware สำหรับจัดการ exceptions แบบรวมศูนย์:

```csharp
public class ExceptionHandlerMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

**ประโยชน์:**
- จัดการ errors ในที่เดียว
- Response format สม่ำเสมอ
- Log errors อัตโนมัติ
- ซ่อน sensitive information

### 7. **Consistent API Responses**

**ไฟล์:** `Models/ApiResponse.cs`

ใช้ wrapper class สำหรับ response:

```csharp
// Success Response
{
    "success": true,
    "message": "ดึงข้อมูลสำเร็จ",
    "data": { ... }
}

// Error Response
{
    "success": false,
    "message": "ข้อมูลไม่ถูกต้อง",
    "statusCode": 400,
    "errors": { ... }
}
```

**ประโยชน์:**
- Client ประมวลผลง่าย
- Format สม่ำเสมอ
- รองรับ error details

### 8. **Dependency Injection**

**ไฟล์:** `Program.cs`

ลงทะเบียน services ผ่าน DI:

```csharp
// Repository - Singleton (เก็บข้อมูลใน memory)
builder.Services.AddSingleton<IProductRepository, ProductRepository>();

// Service - Scoped (สร้างใหม่ทุก request)
builder.Services.AddScoped<IProductService, ProductService>();
```

**ประโยชน์:**
- Loose coupling
- ทดสอบง่ายด้วย mocking
- จัดการ object lifetime อัตโนมัติ

### 9. **Async/Await Pattern**

ทุก method ใช้ async/await:

```csharp
public async Task<ActionResult> GetAllProducts()
{
    var products = await _productService.GetAllProductsAsync();
    return Ok(products);
}
```

**ประโยชน์:**
- ประสิทธิภาพสูง (non-blocking I/O)
- Scalability ดีขึ้น
- จัดการ concurrent requests ได้มากขึ้น

### 10. **Proper HTTP Status Codes**

ใช้ status codes ที่ถูกต้องตามมาตรฐาน:

- `200 OK` - สำเร็จ
- `201 Created` - สร้างสำเร็จ
- `400 Bad Request` - ข้อมูลไม่ถูกต้อง
- `404 Not Found` - ไม่พบข้อมูล
- `500 Internal Server Error` - Server error

## 🚀 วิธีการรันโปรเจค

### 1. รันโปรเจค

```bash
cd ProductsAPI
dotnet run
```

### 2. เข้าถึง Swagger UI

เปิดเบราว์เซอร์และไปที่:
```
http://localhost:5000
```

Swagger UI จะแสดงให้เห็น:
- รายการ endpoints ทั้งหมด
- ข้อมูล request/response schemas
- ทดสอบ API ได้โดยตรง

## 📋 API Endpoints

### Products API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | ดึงสินค้าทั้งหมด |
| GET | `/api/products/{id}` | ดึงสินค้าตาม ID |
| POST | `/api/products` | สร้างสินค้าใหม่ |
| PUT | `/api/products/{id}` | แก้ไขสินค้า |
| DELETE | `/api/products/{id}` | ลบสินค้า |
| GET | `/api/products/search?keyword={keyword}` | ค้นหาสินค้า |
| GET | `/api/products/category/{category}` | ดึงสินค้าตามหมวดหมู่ |

## 📝 ตัวอย่างการใช้งาน

### 1. ดึงสินค้าทั้งหมด (GET)

**Request:**
```bash
curl http://localhost:5000/api/products
```

**Response:**
```json
{
  "success": true,
  "message": "ดึงข้อมูลสินค้าสำเร็จ",
  "data": [
    {
      "id": 1,
      "name": "โน้ตบุ๊ค Dell XPS 13",
      "description": "โน้ตบุ๊คสำหรับการทำงาน พร้อม Intel Core i7",
      "price": 45000,
      "stock": 15,
      "category": "Electronics",
      "isActive": true,
      "isInStock": true,
      "createdAt": "2024-10-13T18:30:00Z"
    }
  ]
}
```

### 2. สร้างสินค้าใหม่ (POST)

**Request:**
```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "iPad Pro 12.9",
    "description": "แท็บเล็ต Apple รุ่นใหม่ล่าสุด",
    "price": 35900,
    "stock": 20,
    "category": "Electronics",
    "isActive": true
  }'
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "สร้างสินค้าสำเร็จ",
  "data": {
    "id": 6,
    "name": "iPad Pro 12.9",
    "description": "แท็บเล็ต Apple รุ่นใหม่ล่าสุด",
    "price": 35900,
    "stock": 20,
    "category": "Electronics",
    "isActive": true,
    "isInStock": true,
    "createdAt": "2024-11-12T18:30:00Z"
  }
}
```

### 3. Validation Error (400 Bad Request)

**Request (ข้อมูลไม่ถูกต้อง):**
```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "",
    "price": -100,
    "stock": -5,
    "category": "InvalidCategory"
  }'
```

**Response:**
```json
{
  "success": false,
  "message": "ข้อมูลที่ส่งมาไม่ถูกต้อง",
  "statusCode": 400,
  "errors": {
    "Name": ["ชื่อสินค้าจำเป็นต้องระบุ"],
    "Price": ["ราคาต้องมากกว่า 0"],
    "Stock": ["จำนวนสินค้าต้องมากกว่าหรือเท่ากับ 0"],
    "Category": ["หมวดหมู่ไม่ถูกต้อง ต้องเป็น: Electronics, Clothing, Books, Food, Toys, Other"]
  }
}
```

### 4. Not Found (404)

**Request:**
```bash
curl http://localhost:5000/api/products/999
```

**Response:**
```json
{
  "success": false,
  "message": "ไม่พบสินค้า ID: 999",
  "statusCode": 404
}
```

## 🧪 การทดสอบด้วย Swagger UI

1. เปิด http://localhost:5000
2. เลือก endpoint ที่ต้องการทดสอบ
3. คลิก "Try it out"
4. กรอกข้อมูล
5. คลิก "Execute"
6. ดูผลลัพธ์ใน Response section

## 🔧 หมวดหมู่สินค้าที่รองรับ

- `Electronics` - อิเล็กทรอนิกส์
- `Clothing` - เสื้อผ้า
- `Books` - หนังสือ
- `Food` - อาหาร
- `Toys` - ของเล่น
- `Other` - อื่นๆ

## 📦 Dependencies ที่ใช้

```xml
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.1" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="12.1.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="10.0.0" />
```

## 🎓 สิ่งที่เรียนรู้จากโปรเจคนี้

✅ **Architecture Patterns**
- Repository Pattern
- Service Layer Pattern
- Dependency Injection Pattern

✅ **Best Practices**
- DTOs for API contracts
- FluentValidation for input validation
- Global exception handling
- Consistent API responses
- Async/await programming

✅ **API Design**
- RESTful principles
- Proper HTTP status codes
- API documentation with Swagger
- CORS configuration

✅ **Code Quality**
- Separation of concerns
- Single responsibility principle
- Interface-based programming
- Dependency injection

## 🔄 ขั้นตอนต่อไป (ขยายโปรเจค)

1. **เพิ่ม Database**
   - ใช้ Entity Framework Core
   - เชื่อมต่อ SQL Server/PostgreSQL
   - Implement Migrations

2. **Authentication & Authorization**
   - JWT tokens
   - Role-based access control
   - User management

3. **Advanced Features**
   - Pagination
   - Sorting
   - Filtering
   - Caching (Redis)

4. **Testing**
   - Unit tests
   - Integration tests
   - API tests with xUnit

5. **Deployment**
   - Containerization with Docker
   - CI/CD pipelines
   - Cloud deployment (Azure/AWS)

## 💡 เคล็ดลับสำหรับมือใหม่

1. **เริ่มจาก Simple → Complex**
   - เข้าใจ basic CRUD ก่อน
   - ค่อยๆ เพิ่ม features

2. **ใช้ Swagger สำหรับทดสอบ**
   - สะดวกกว่า Postman ในระหว่างพัฒนา
   - Documentation ออกมาพร้อมกัน

3. **Log ทุกอย่าง**
   - ดู logs เพื่อ debug
   - เข้าใจ flow ของ requests

4. **อ่าน Error Messages**
   - Validation errors บอกปัญหาชัดเจน
   - Stack traces ช่วย debug

5. **ศึกษา HTTP Status Codes**
   - ใช้ให้ถูกต้องตามมาตรฐาน
   - ช่วยให้ API มืออาชีพขึ้น

## 📚 แหล่งเรียนรู้เพิ่มเติม

- [Microsoft Docs - Web APIs](https://docs.microsoft.com/aspnet/core/web-api)
- [RESTful API Design Best Practices](https://restfulapi.net/)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [Repository Pattern](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)

## ❓ คำถามที่พบบ่อย

**Q: ทำไมต้องใช้ DTOs?**
A: เพื่อแยก API contract จาก internal models และควบคุมข้อมูลที่ส่งเข้า/ออกได้

**Q: Repository Pattern จำเป็นไหม?**
A: สำหรับโปรเจคเล็ก อาจไม่จำเป็น แต่สำหรับโปรเจคใหญ่ ช่วยให้ maintain ง่ายขึ้นมาก

**Q: FluentValidation ดีกว่า Data Annotations ไหม?**
A: ขึ้นอยู่กับความซับซ้อน FluentValidation ยืดหยุ่นกว่าและทดสอบง่ายกว่า

**Q: จะเปลี่ยนจาก In-Memory เป็น Database ยังไง?**
A: แค่เปลี่ยน Implementation ของ IProductRepository เป็น EF Core repository

---

**สร้างโดย:** DotNetCore-Book Project  
**อัปเดตล่าสุด:** November 2024  
**เวอร์ชัน:** .NET 9.0
