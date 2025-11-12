# บทที่ 9: การทดสอบ (Testing) ใน .NET Core

## 9.1 ประเภทของการทดสอบ

### 9.1.1 Unit Testing
- ทดสอบ individual methods/functions
- แยกจาก dependencies
- รันเร็ว, ควรมีจำนวนมาก

### 9.1.2 Integration Testing
- ทดสอบการทำงานร่วมกันของหลาย components
- ทดสอบกับ database, external services
- รันช้ากว่า unit tests

### 9.1.3 End-to-End Testing
- ทดสอบระบบทั้งหมด
- ทดสอบ user scenarios
- รันช้าที่สุด

## 9.2 Unit Testing ด้วย xUnit

### 9.2.1 สร้าง Test Project

```bash
# สร้าง solution
dotnet new sln -n MyShop

# สร้าง main project
dotnet new classlib -n MyShop.Core
dotnet sln add MyShop.Core

# สร้าง test project
dotnet new xunit -n MyShop.Tests
dotnet sln add MyShop.Tests

# เพิ่ม reference
cd MyShop.Tests
dotnet add reference ../MyShop.Core/MyShop.Core.csproj
```

### 9.2.2 เขียน Unit Test แรก

สร้าง Class ที่ต้องการทดสอบ `MyShop.Core/Calculator.cs`:

```csharp
namespace MyShop.Core
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Subtract(int a, int b)
        {
            return a - b;
        }

        public int Multiply(int a, int b)
        {
            return a * b;
        }

        public double Divide(int a, int b)
        {
            if (b == 0)
                throw new DivideByZeroException("Cannot divide by zero");
            
            return (double)a / b;
        }
    }
}
```

สร้าง Test `MyShop.Tests/CalculatorTests.cs`:

```csharp
using Xunit;
using MyShop.Core;

namespace MyShop.Tests
{
    public class CalculatorTests
    {
        private readonly Calculator _calculator;

        public CalculatorTests()
        {
            _calculator = new Calculator();
        }

        [Fact]
        public void Add_TwoPositiveNumbers_ReturnsCorrectSum()
        {
            // Arrange
            int a = 5;
            int b = 3;
            int expected = 8;

            // Act
            int result = _calculator.Add(a, b);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Subtract_TwoNumbers_ReturnsCorrectDifference()
        {
            // Arrange
            int a = 10;
            int b = 3;

            // Act
            int result = _calculator.Subtract(a, b);

            // Assert
            Assert.Equal(7, result);
        }

        [Theory]
        [InlineData(2, 3, 6)]
        [InlineData(5, 4, 20)]
        [InlineData(-2, 3, -6)]
        [InlineData(0, 5, 0)]
        public void Multiply_VariousInputs_ReturnsCorrectProduct(int a, int b, int expected)
        {
            // Act
            int result = _calculator.Multiply(a, b);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Divide_ByZero_ThrowsException()
        {
            // Arrange
            int a = 10;
            int b = 0;

            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => _calculator.Divide(a, b));
        }

        [Theory]
        [InlineData(10, 2, 5.0)]
        [InlineData(7, 2, 3.5)]
        [InlineData(-10, 2, -5.0)]
        public void Divide_ValidInputs_ReturnsCorrectQuotient(int a, int b, double expected)
        {
            // Act
            double result = _calculator.Divide(a, b);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
```

### 9.2.3 รัน Tests

```bash
# รัน tests ทั้งหมด
dotnet test

# รันพร้อม code coverage
dotnet test --collect:"XPlat Code Coverage"

# รัน specific test
dotnet test --filter "FullyQualifiedName~CalculatorTests"
```

## 9.3 Mocking ด้วย Moq

### 9.3.1 ติดตั้ง Moq

```bash
dotnet add package Moq
```

### 9.3.2 สร้าง Service และ Test

Service Interface:

```csharp
namespace MyShop.Core.Services
{
    public interface IProductService
    {
        Task<Product?> GetProductAsync(int id);
        Task<List<Product>> GetAllProductsAsync();
        Task<bool> CreateProductAsync(Product product);
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
```

Business Logic:

```csharp
namespace MyShop.Core.Services
{
    public class OrderService
    {
        private readonly IProductService _productService;

        public OrderService(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<decimal> CalculateOrderTotalAsync(List<int> productIds)
        {
            decimal total = 0;

            foreach (var id in productIds)
            {
                var product = await _productService.GetProductAsync(id);
                if (product != null)
                {
                    total += product.Price;
                }
            }

            return total;
        }

        public async Task<bool> IsProductAvailableAsync(int productId)
        {
            var product = await _productService.GetProductAsync(productId);
            return product != null;
        }
    }
}
```

Test with Mocking:

```csharp
using Xunit;
using Moq;
using MyShop.Core.Services;

namespace MyShop.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockProductService = new Mock<IProductService>();
            _orderService = new OrderService(_mockProductService.Object);
        }

        [Fact]
        public async Task CalculateOrderTotal_WithValidProducts_ReturnsCorrectTotal()
        {
            // Arrange
            var productIds = new List<int> { 1, 2, 3 };
            
            _mockProductService.Setup(x => x.GetProductAsync(1))
                .ReturnsAsync(new Product { Id = 1, Name = "Product 1", Price = 100 });
            
            _mockProductService.Setup(x => x.GetProductAsync(2))
                .ReturnsAsync(new Product { Id = 2, Name = "Product 2", Price = 200 });
            
            _mockProductService.Setup(x => x.GetProductAsync(3))
                .ReturnsAsync(new Product { Id = 3, Name = "Product 3", Price = 300 });

            // Act
            var total = await _orderService.CalculateOrderTotalAsync(productIds);

            // Assert
            Assert.Equal(600, total);
            _mockProductService.Verify(x => x.GetProductAsync(It.IsAny<int>()), Times.Exactly(3));
        }

        [Fact]
        public async Task CalculateOrderTotal_WithNullProduct_IgnoresNull()
        {
            // Arrange
            var productIds = new List<int> { 1, 2 };
            
            _mockProductService.Setup(x => x.GetProductAsync(1))
                .ReturnsAsync(new Product { Id = 1, Name = "Product 1", Price = 100 });
            
            _mockProductService.Setup(x => x.GetProductAsync(2))
                .ReturnsAsync((Product?)null);

            // Act
            var total = await _orderService.CalculateOrderTotalAsync(productIds);

            // Assert
            Assert.Equal(100, total);
        }

        [Fact]
        public async Task IsProductAvailable_ProductExists_ReturnsTrue()
        {
            // Arrange
            _mockProductService.Setup(x => x.GetProductAsync(1))
                .ReturnsAsync(new Product { Id = 1, Name = "Product 1", Price = 100 });

            // Act
            var result = await _orderService.IsProductAvailableAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsProductAvailable_ProductNotExists_ReturnsFalse()
        {
            // Arrange
            _mockProductService.Setup(x => x.GetProductAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _orderService.IsProductAvailableAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
```

## 9.4 Integration Testing

### 9.4.1 ติดตั้ง Packages

```bash
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

### 9.4.2 สร้าง WebApplicationFactory

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyWebApp.Data;

namespace MyShop.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove real database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add InMemory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDb");
                });

                // Seed test data
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                db.Database.EnsureCreated();
                SeedTestData(db);
            });
        }

        private void SeedTestData(ApplicationDbContext db)
        {
            db.Products.AddRange(
                new Product { Id = 1, Name = "Test Product 1", Price = 100 },
                new Product { Id = 2, Name = "Test Product 2", Price = 200 }
            );
            db.SaveChanges();
        }
    }
}
```

### 9.4.3 Integration Test Example

```csharp
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace MyShop.IntegrationTests
{
    public class ProductApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProducts_ReturnsSuccessAndProducts()
        {
            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadFromJsonAsync<List<Product>>();
            
            Assert.NotNull(products);
            Assert.NotEmpty(products);
        }

        [Fact]
        public async Task GetProduct_WithValidId_ReturnsProduct()
        {
            // Act
            var response = await _client.GetAsync("/api/products/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var product = await response.Content.ReadFromJsonAsync<Product>();
            
            Assert.NotNull(product);
            Assert.Equal(1, product.Id);
        }

        [Fact]
        public async Task GetProduct_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/products/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_WithValidData_ReturnsCreated()
        {
            // Arrange
            var newProduct = new Product
            {
                Name = "New Test Product",
                Price = 500
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", newProduct);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
            
            Assert.NotNull(createdProduct);
            Assert.Equal(newProduct.Name, createdProduct.Name);
        }
    }
}
```

## 9.5 Test Best Practices

### 9.5.1 Naming Convention

```csharp
// Pattern: MethodName_Scenario_ExpectedResult
[Fact]
public void Add_TwoPositiveNumbers_ReturnsSum()
{
}

[Fact]
public void GetProduct_InvalidId_ThrowsNotFoundException()
{
}
```

### 9.5.2 AAA Pattern

```csharp
[Fact]
public void TestMethod()
{
    // Arrange - Setup test data
    var input = 5;
    var expected = 10;

    // Act - Execute the method
    var result = _service.DoSomething(input);

    // Assert - Verify the result
    Assert.Equal(expected, result);
}
```

### 9.5.3 One Assert Per Test

```csharp
// ✅ Good
[Fact]
public void Product_HasValidName()
{
    var product = new Product { Name = "Test" };
    Assert.NotNull(product.Name);
}

[Fact]
public void Product_HasValidPrice()
{
    var product = new Product { Price = 100 };
    Assert.True(product.Price > 0);
}

// ❌ Bad
[Fact]
public void Product_IsValid()
{
    var product = new Product { Name = "Test", Price = 100 };
    Assert.NotNull(product.Name);
    Assert.True(product.Price > 0);
}
```

## 9.6 Test Coverage

```bash
# Install report generator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
reportgenerator \
    -reports:"**/coverage.cobertura.xml" \
    -targetdir:"coveragereport" \
    -reporttypes:Html

# Open report
open coveragereport/index.html
```

## 9.7 สรุป

ในบทนี้เราได้เรียนรู้:

✅ ประเภทของการทดสอบ
✅ Unit Testing ด้วย xUnit
✅ Mocking ด้วย Moq
✅ Integration Testing
✅ Test Best Practices
✅ Test Coverage

## 📚 แบบฝึกหัด

1. เขียน Unit Tests สำหรับ ProductService
2. สร้าง Integration Tests สำหรับ Web API
3. ใช้ Moq ทดสอบ Service ที่มี multiple dependencies
4. วัด Code Coverage ของโปรเจค
5. เขียน Tests สำหรับ Edge Cases

---

**ก่อนหน้า:** [บทที่ 8 - Dependency Injection](../Chapter08-DependencyInjection/README.md)
**ต่อไป:** [บทที่ 10 - การ Deploy และ Best Practices](../Chapter10-Deployment/README.md)
