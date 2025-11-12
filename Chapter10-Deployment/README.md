# บทที่ 10: การ Deploy และ Best Practices

## 10.1 การ Publish Application

### 10.1.1 Framework-Dependent Deployment

```bash
# Publish แบบต้องมี .NET Runtime บนเครื่อง target
dotnet publish -c Release -o ./publish

# ขนาดเล็กกว่า แต่ต้องติดตั้ง .NET Runtime ก่อน
```

### 10.1.2 Self-Contained Deployment

```bash
# Publish พร้อม Runtime (ไม่ต้องติดตั้ง .NET)
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish

# Runtime Identifiers (RID):
# - win-x64: Windows 64-bit
# - linux-x64: Linux 64-bit
# - osx-x64: macOS 64-bit
```

### 10.1.3 Single File Deployment

```bash
# สร้าง executable file เดียว
dotnet publish -c Release -r win-x64 \
    --self-contained true \
    -p:PublishSingleFile=true \
    -o ./publish
```

### 10.1.4 Trimming

```bash
# ลดขนาดโดยตัดส่วนที่ไม่ใช้ออก
dotnet publish -c Release -r win-x64 \
    --self-contained true \
    -p:PublishTrimmed=true \
    -o ./publish
```

## 10.2 Docker Deployment

### 10.2.1 สร้าง Dockerfile

สร้างไฟล์ `Dockerfile`:

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["MyWebApp/MyWebApp.csproj", "MyWebApp/"]
RUN dotnet restore "MyWebApp/MyWebApp.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/MyWebApp"
RUN dotnet build "MyWebApp.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "MyWebApp.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "MyWebApp.dll"]
```

### 10.2.2 สร้าง .dockerignore

```
**/.dockerignore
**/.git
**/.gitignore
**/.vs
**/.vscode
**/bin
**/obj
**/.toolstarget
```

### 10.2.3 Build และ Run Docker Image

```bash
# Build image
docker build -t mywebapp:latest .

# Run container
docker run -d -p 8080:80 --name myapp mywebapp:latest

# Check logs
docker logs myapp

# Stop container
docker stop myapp

# Remove container
docker rm myapp
```

### 10.2.4 Docker Compose

สร้างไฟล์ `docker-compose.yml`:

```yaml
version: '3.8'

services:
  web:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=MyWebApp;User=sa;Password=Your_password123
    depends_on:
      - db
    networks:
      - mynetwork

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Your_password123
      - MSSQL_PID=Express
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    networks:
      - mynetwork

volumes:
  sqldata:

networks:
  mynetwork:
```

```bash
# Run with docker-compose
docker-compose up -d

# Stop
docker-compose down

# View logs
docker-compose logs -f
```

## 10.3 Cloud Deployment

### 10.3.1 Deploy to Azure App Service

```bash
# Login to Azure
az login

# Create resource group
az group create --name MyResourceGroup --location southeastasia

# Create App Service plan
az appservice plan create \
    --name MyAppServicePlan \
    --resource-group MyResourceGroup \
    --sku B1 \
    --is-linux

# Create Web App
az webapp create \
    --name MyUniqueWebApp \
    --resource-group MyResourceGroup \
    --plan MyAppServicePlan \
    --runtime "DOTNET|8.0"

# Deploy
dotnet publish -c Release
cd bin/Release/net8.0/publish
zip -r ../publish.zip .
az webapp deployment source config-zip \
    --resource-group MyResourceGroup \
    --name MyUniqueWebApp \
    --src ../publish.zip
```

### 10.3.2 Deploy to Azure Container Instances

```bash
# Build and push to Azure Container Registry
az acr create --resource-group MyResourceGroup \
    --name myregistry --sku Basic

az acr login --name myregistry

docker tag mywebapp:latest myregistry.azurecr.io/mywebapp:latest
docker push myregistry.azurecr.io/mywebapp:latest

# Deploy to ACI
az container create \
    --resource-group MyResourceGroup \
    --name mywebapp-container \
    --image myregistry.azurecr.io/mywebapp:latest \
    --dns-name-label mywebapp-unique \
    --ports 80
```

## 10.4 Configuration Management

### 10.4.1 appsettings.json Hierarchy

```
appsettings.json              # Base configuration
appsettings.Development.json  # Development overrides
appsettings.Production.json   # Production overrides
```

**appsettings.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyDb;Trusted_Connection=true"
  },
  "AppSettings": {
    "ApplicationName": "My Web App",
    "MaxUploadSize": 5242880
  }
}
```

**appsettings.Production.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=MyDb;User=sa;Password=xxx"
  }
}
```

### 10.4.2 Environment Variables

```bash
# Set environment
export ASPNETCORE_ENVIRONMENT=Production

# Connection string from environment
export ConnectionStrings__DefaultConnection="Server=prod;Database=MyDb"

# Custom settings
export AppSettings__MaxUploadSize=10485760
```

### 10.4.3 User Secrets (Development)

```bash
# Initialize user secrets
dotnet user-secrets init

# Set secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=dev;Database=MyDb"
dotnet user-secrets set "ApiKeys:OpenAI" "sk-xxxxx"

# List secrets
dotnet user-secrets list

# Clear secrets
dotnet user-secrets clear
```

## 10.5 Logging และ Monitoring

### 10.5.1 Built-in Logging

```csharp
public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Accessing products page");
        
        try
        {
            // Code here
            _logger.LogDebug("Debug information");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred");
            throw;
        }

        return View();
    }
}
```

### 10.5.2 Serilog

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console
```

```csharp
// Program.cs
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Rest of the code...

Log.Information("Starting web application");
app.Run();
```

### 10.5.3 Application Insights (Azure)

```bash
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(
    builder.Configuration["ApplicationInsights:ConnectionString"]);
```

## 10.6 Security Best Practices

### 10.6.1 HTTPS Enforcement

```csharp
// Program.cs
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
```

### 10.6.2 CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy
            .WithOrigins("https://myapp.com")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

app.UseCors("AllowSpecificOrigin");
```

### 10.6.3 Data Protection

```csharp
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))
    .SetApplicationName("MyApp");
```

### 10.6.4 Input Validation

```csharp
public class CreateProductDto
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Name { get; set; }

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    [RegularExpression(@"^\d{13}$")]
    public string Barcode { get; set; }
}
```

### 10.6.5 SQL Injection Prevention

```csharp
// ✅ Good - Parameterized query
var products = await _context.Products
    .Where(p => p.Name == searchTerm)
    .ToListAsync();

// ❌ Bad - String concatenation
var query = $"SELECT * FROM Products WHERE Name = '{searchTerm}'";
```

## 10.7 Performance Best Practices

### 10.7.1 Response Caching

```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();

[ResponseCache(Duration = 60)]
public IActionResult Index()
{
    return View();
}
```

### 10.7.2 Memory Caching

```csharp
builder.Services.AddMemoryCache();

public class ProductService
{
    private readonly IMemoryCache _cache;

    public async Task<List<Product>> GetProductsAsync()
    {
        if (!_cache.TryGetValue("products", out List<Product> products))
        {
            products = await _context.Products.ToListAsync();
            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));
            
            _cache.Set("products", products, cacheOptions);
        }

        return products;
    }
}
```

### 10.7.3 Response Compression

```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

app.UseResponseCompression();
```

### 10.7.4 Database Query Optimization

```csharp
// ✅ Good - AsNoTracking for read-only
var products = await _context.Products
    .AsNoTracking()
    .ToListAsync();

// ✅ Good - Select only needed fields
var productNames = await _context.Products
    .Select(p => new { p.Id, p.Name })
    .ToListAsync();

// ❌ Bad - Loading unnecessary data
var products = await _context.Products
    .Include(p => p.Category)
    .Include(p => p.Reviews)
    .ToListAsync();
```

## 10.8 Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddUrlGroup(new Uri("https://api.example.com"), "External API");

app.MapHealthChecks("/health");
```

## 10.9 CI/CD Pipeline

### 10.9.1 GitHub Actions

สร้างไฟล์ `.github/workflows/dotnet.yml`:

```yaml
name: .NET Core CI/CD

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore --configuration Release
      
    - name: Test
      run: dotnet test --no-build --verbosity normal
      
    - name: Publish
      run: dotnet publish -c Release -o ./publish
      
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'my-web-app'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

## 10.10 สรุป

ในบทนี้เราได้เรียนรู้:

✅ วิธีการ Publish และ Deploy แอปพลิเคชัน
✅ Docker และ Containerization
✅ Cloud Deployment (Azure)
✅ Configuration Management
✅ Logging และ Monitoring
✅ Security Best Practices
✅ Performance Optimization
✅ Health Checks
✅ CI/CD Pipeline

## 📚 Checklist ก่อน Deploy Production

- [ ] ทดสอบโค้ดครบถ้วน (Unit Tests, Integration Tests)
- [ ] ตั้งค่า Environment เป็น Production
- [ ] ใช้ HTTPS และ HSTS
- [ ] ตั้งค่า CORS ให้ถูกต้อง
- [ ] Validate input ทุกจุด
- [ ] ใช้ Parameterized queries
- [ ] เปิด Logging และ Monitoring
- [ ] ตั้งค่า Error handling
- [ ] เปิด Response Compression
- [ ] ตั้งค่า Caching
- [ ] ทำ Health Checks
- [ ] Backup database
- [ ] ทดสอบ Deployment process
- [ ] เตรียม Rollback plan
- [ ] Document deployment steps

## 🎓 สรุปท้ายหนังสือ

ยินดีด้วย! คุณได้เรียนรู้พื้นฐานและเทคนิคสำคัญของ .NET Core แล้ว

**สิ่งที่เรียนรู้:**
- .NET Core Fundamentals
- C# Programming
- Console Applications
- ASP.NET Core MVC
- Entity Framework Core
- Web APIs
- Dependency Injection
- Testing
- Deployment

**ขั้นต่อไป:**
- สร้างโปรเจคจริง
- ศึกษา Advanced topics (SignalR, Blazor, gRPC)
- เรียนรู้ Design Patterns
- ปรับปรุง Security
- เพิ่ม Performance
- ติดตามเทคโนโลยีใหม่ๆ

**แหล่งเรียนรู้เพิ่มเติม:**
- [Microsoft Docs](https://docs.microsoft.com/dotnet)
- [.NET Blog](https://devblogs.microsoft.com/dotnet)
- [GitHub .NET Repository](https://github.com/dotnet)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/.net-core)

---

**ก่อนหน้า:** [บทที่ 9 - การทดสอบ](../Chapter09-Testing/README.md)
**กลับไปที่:** [สารบัญ](../README.md)
