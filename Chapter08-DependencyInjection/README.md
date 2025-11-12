# บทที่ 8: Dependency Injection

## 8.1 Dependency Injection คืออะไร

Dependency Injection (DI) เป็น design pattern ที่ช่วยให้ code มีความยืดหยุ่นและทดสอบง่ายขึ้น โดยการ "ฉีด" dependencies เข้าไปใน class แทนที่จะสร้างเอง

### ประโยชน์:
- **Loose Coupling**: แยก dependencies ออกจากกัน
- **Testability**: ทดสอบง่ายด้วย mock objects
- **Maintainability**: แก้ไข code ง่ายขึ้น
- **Reusability**: ใช้ซ้ำได้ง่าย

## 8.2 Service Lifetimes

### 8.2.1 Transient

สร้าง instance ใหม่ทุกครั้งที่ request

```csharp
builder.Services.AddTransient<IEmailService, EmailService>();
```

**เหมาะสำหรับ:**
- Lightweight, stateless services
- Services ที่ไม่ share state

### 8.2.2 Scoped

สร้าง instance ใหม่ต่อ HTTP request

```csharp
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ApplicationDbContext>();
```

**เหมาะสำหรับ:**
- Database contexts
- Services ที่ต้องการ share state ภายใน request เดียว

### 8.2.3 Singleton

สร้าง instance เดียวตลอด lifetime ของ application

```csharp
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
```

**เหมาะสำหรับ:**
- Configuration objects
- Logging services
- Caching services

## 8.3 การใช้งาน Dependency Injection

### 8.3.1 สร้าง Interface และ Implementation

สร้างไฟล์ `Services/IProductService.cs`:

```csharp
namespace MyApp.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}
```

สร้างไฟล์ `Services/ProductService.cs`:

```csharp
using MyApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            ApplicationDbContext context,
            ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            _logger.LogInformation("Getting all products");
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Getting product with ID: {Id}", id);
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _logger.LogInformation("Creating new product: {Name}", product.Name);
            
            product.CreatedDate = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            
            return product;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            _logger.LogInformation("Updating product ID: {Id}", product.Id);
            
            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogInformation("Deleting product ID: {Id}", id);
            
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
```

### 8.3.2 Register Services

ใน `Program.cs`:

```csharp
// Register services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
```

### 8.3.3 Inject ใน Controller

```csharp
using Microsoft.AspNetCore.Mvc;
using MyApp.Services;

namespace MyApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        // Constructor Injection
        public ProductController(
            IProductService productService,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productService.CreateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }
    }
}
```

## 8.4 Multiple Implementations

### 8.4.1 สร้าง Multiple Implementations

```csharp
public interface INotificationService
{
    Task SendAsync(string message);
}

public class EmailNotificationService : INotificationService
{
    public async Task SendAsync(string message)
    {
        // Send email
        await Task.CompletedTask;
    }
}

public class SmsNotificationService : INotificationService
{
    public async Task SendAsync(string message)
    {
        // Send SMS
        await Task.CompletedTask;
    }
}

public class LineNotificationService : INotificationService
{
    public async Task SendAsync(string message)
    {
        // Send LINE notification
        await Task.CompletedTask;
    }
}
```

### 8.4.2 Register และใช้งาน

```csharp
// Register all implementations
builder.Services.AddScoped<INotificationService, EmailNotificationService>();
builder.Services.AddScoped<INotificationService, SmsNotificationService>();
builder.Services.AddScoped<INotificationService, LineNotificationService>();

// Inject ทั้งหมด
public class NotificationController : Controller
{
    private readonly IEnumerable<INotificationService> _notificationServices;

    public NotificationController(IEnumerable<INotificationService> notificationServices)
    {
        _notificationServices = notificationServices;
    }

    public async Task<IActionResult> SendNotification(string message)
    {
        foreach (var service in _notificationServices)
        {
            await service.SendAsync(message);
        }

        return Ok();
    }
}
```

## 8.5 Options Pattern

### 8.5.1 สร้าง Configuration Class

```csharp
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
}
```

### 8.5.2 Configure ใน appsettings.json

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "myapp@example.com",
    "Password": "password",
    "FromEmail": "noreply@example.com"
  }
}
```

### 8.5.3 Register และ Inject Options

```csharp
// Program.cs
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Service
using Microsoft.Extensions.Options;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> settings,
        ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Sending email to {To}", to);
        _logger.LogInformation("SMTP Server: {Server}:{Port}", 
            _settings.SmtpServer, _settings.SmtpPort);
        
        // Implementation here
        await Task.CompletedTask;
    }
}
```

## 8.6 Factory Pattern

### 8.6.1 สร้าง Factory

```csharp
public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(decimal amount);
}

public class CreditCardPaymentService : IPaymentService
{
    public async Task<bool> ProcessPaymentAsync(decimal amount)
    {
        // Process credit card payment
        return await Task.FromResult(true);
    }
}

public class PayPalPaymentService : IPaymentService
{
    public async Task<bool> ProcessPaymentAsync(decimal amount)
    {
        // Process PayPal payment
        return await Task.FromResult(true);
    }
}

public interface IPaymentServiceFactory
{
    IPaymentService CreatePaymentService(string paymentMethod);
}

public class PaymentServiceFactory : IPaymentServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPaymentService CreatePaymentService(string paymentMethod)
    {
        return paymentMethod.ToLower() switch
        {
            "creditcard" => _serviceProvider.GetRequiredService<CreditCardPaymentService>(),
            "paypal" => _serviceProvider.GetRequiredService<PayPalPaymentService>(),
            _ => throw new ArgumentException("Invalid payment method")
        };
    }
}

// Register
builder.Services.AddScoped<CreditCardPaymentService>();
builder.Services.AddScoped<PayPalPaymentService>();
builder.Services.AddScoped<IPaymentServiceFactory, PaymentServiceFactory>();
```

## 8.7 Unit of Work Pattern

```csharp
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IProductRepository? _products;
    private ICategoryRepository? _categories;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products =>
        _products ??= new ProductRepository(_context);

    public ICategoryRepository Categories =>
        _categories ??= new CategoryRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

// Register
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Usage
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> CreateProductAsync(Product product)
    {
        _unitOfWork.Products.Add(product);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
```

## 8.8 สรุป

ในบทนี้เราได้เรียนรู้:

✅ Dependency Injection คืออะไร
✅ Service Lifetimes (Transient, Scoped, Singleton)
✅ การสร้างและ Register Services
✅ Constructor Injection
✅ Options Pattern
✅ Factory Pattern
✅ Unit of Work Pattern

## 📚 แบบฝึกหัด

1. สร้าง ILoggerService และ implement 2 แบบ (File, Database)
2. ใช้ Options Pattern กับ Application Settings
3. สร้าง Factory สำหรับ Report Services
4. Implement Repository Pattern
5. สร้าง Middleware ด้วย DI

---

**ก่อนหน้า:** [บทที่ 7 - Web API ด้วย ASP.NET Core](../Chapter07-WebAPI/README.md)
**ต่อไป:** [บทที่ 9 - การทดสอบ](../Chapter09-Testing/README.md)
