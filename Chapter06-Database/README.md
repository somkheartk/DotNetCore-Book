# บทที่ 6: การทำงานกับฐานข้อมูล (Entity Framework Core)

## 6.1 แนะนำ Entity Framework Core

Entity Framework Core (EF Core) เป็น Object-Relational Mapper (ORM) ที่ช่วยให้เราทำงานกับฐานข้อมูลผ่าน C# objects แทนการเขียน SQL โดยตรง

### คุณสมบัติสำคัญ:
- **Cross-platform**: รองรับหลายฐานข้อมูล (SQL Server, PostgreSQL, MySQL, SQLite)
- **LINQ Support**: Query ข้อมูลด้วย LINQ
- **Code First**: สร้างฐานข้อมูลจาก C# classes
- **Migrations**: จัดการ schema changes
- **Change Tracking**: ติดตามการเปลี่ยนแปลงข้อมูล

## 6.2 การติดตั้งและตั้งค่า

### 6.2.1 ติดตั้ง Packages

```bash
# สร้างโปรเจค
dotnet new mvc -n MyShopApp
cd MyShopApp

# ติดตั้ง EF Core packages
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design

# สำหรับใช้ SQLite (ทดสอบง่ายกว่า)
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### 6.2.2 สร้าง Models

สร้างไฟล์ `Models/Category.cs`:

```csharp
using System.ComponentModel.DataAnnotations;

namespace MyShopApp.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
```

สร้างไฟล์ `Models/Product.cs`:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyShopApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign Key
        public int CategoryId { get; set; }

        // Navigation property
        public Category Category { get; set; } = null!;
    }
}
```

### 6.2.3 สร้าง DbContext

สร้างไฟล์ `Data/ApplicationDbContext.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using MyShopApp.Models;

namespace MyShopApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "อิเล็กทรอนิกส์", Description = "สินค้าอิเล็กทรอนิกส์" },
                new Category { Id = 2, Name = "เสื้อผ้า", Description = "เสื้อผ้าแฟชั่น" },
                new Category { Id = 3, Name = "หนังสือ", Description = "หนังสือและนิตยสาร" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product 
                { 
                    Id = 1, 
                    Name = "โน้ตบุ๊ค Dell XPS", 
                    Description = "โน้ตบุ๊คสเปกสูง",
                    Price = 45000, 
                    Stock = 10,
                    CategoryId = 1,
                    CreatedDate = DateTime.Now
                },
                new Product 
                { 
                    Id = 2, 
                    Name = "เสื้อยืดสีขาว", 
                    Description = "เสื้อยืดคอกลม 100% cotton",
                    Price = 299, 
                    Stock = 50,
                    CategoryId = 2,
                    CreatedDate = DateTime.Now
                }
            );
        }
    }
}
```

### 6.2.4 Configure Connection String

แก้ไขไฟล์ `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=myshop.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 6.2.5 Register DbContext

แก้ไขไฟล์ `Program.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using MyShopApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Auto migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

## 6.3 Migrations

### 6.3.1 สร้าง Initial Migration

```bash
# สร้าง migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# ลบ migration (ถ้าต้องการ)
dotnet ef migrations remove

# ดู SQL ที่จะรัน
dotnet ef migrations script
```

### 6.3.2 เพิ่ม Migration ใหม่

```bash
# เมื่อมีการแก้ไข model
dotnet ef migrations add AddImageUrlToProduct
dotnet ef database update
```

## 6.4 CRUD Operations

### 6.4.1 Controller with EF Core

สร้างไฟล์ `Controllers/ProductController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyShopApp.Data;
using MyShopApp.Models;

namespace MyShopApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index(string searchTerm, int? categoryId)
        {
            // Query with Include for eager loading
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await query
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;

            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Stock,CategoryId,ImageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "เพิ่มสินค้าเรียบร้อยแล้ว";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Stock,CategoryId,ImageUrl,CreatedDate")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "แก้ไขสินค้าเรียบร้อยแล้ว";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "ลบสินค้าเรียบร้อยแล้ว";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
```

## 6.5 Advanced Queries

### 6.5.1 Complex LINQ Queries

```csharp
// ดูสินค้าทั้งหมดพร้อม Category (Eager Loading)
var products = await _context.Products
    .Include(p => p.Category)
    .ToListAsync();

// Explicit Loading
var product = await _context.Products.FindAsync(id);
await _context.Entry(product).Reference(p => p.Category).LoadAsync();

// Lazy Loading (ต้องติดตั้ง Microsoft.EntityFrameworkCore.Proxies)
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseLazyLoadingProxies().UseSqlite(...));

// Projection
var productList = await _context.Products
    .Select(p => new
    {
        p.Id,
        p.Name,
        p.Price,
        CategoryName = p.Category.Name
    })
    .ToListAsync();

// Grouping
var productsByCategory = await _context.Products
    .GroupBy(p => p.Category.Name)
    .Select(g => new
    {
        Category = g.Key,
        Count = g.Count(),
        TotalValue = g.Sum(p => p.Price * p.Stock)
    })
    .ToListAsync();

// Filtering with multiple conditions
var filteredProducts = await _context.Products
    .Where(p => p.Price >= 1000 && p.Price <= 50000)
    .Where(p => p.Stock > 0)
    .OrderBy(p => p.Price)
    .Take(10)
    .ToListAsync();

// Pagination
int pageSize = 10;
int pageNumber = 1;
var paginatedProducts = await _context.Products
    .OrderByDescending(p => p.CreatedDate)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### 6.5.2 Raw SQL Queries

```csharp
// FromSqlRaw
var products = await _context.Products
    .FromSqlRaw("SELECT * FROM Products WHERE Price > {0}", 1000)
    .ToListAsync();

// FromSqlInterpolated (safer)
decimal minPrice = 1000;
var products2 = await _context.Products
    .FromSqlInterpolated($"SELECT * FROM Products WHERE Price > {minPrice}")
    .ToListAsync();

// Execute SQL commands
int rowsAffected = await _context.Database
    .ExecuteSqlRawAsync("UPDATE Products SET Stock = Stock + 10 WHERE CategoryId = {0}", 1);
```

## 6.6 Relationships

### 6.6.1 One-to-Many

```csharp
// One Category has Many Products
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }  // Foreign Key
    public Category Category { get; set; }
}
```

### 6.6.2 Many-to-Many

```csharp
// Product can have many Tags, Tag can be on many Products
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<ProductTag> ProductTags { get; set; }
}

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<ProductTag> ProductTags { get; set; }
}

public class ProductTag
{
    public int ProductId { get; set; }
    public Product Product { get; set; }
    
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}

// Configure in DbContext
modelBuilder.Entity<ProductTag>()
    .HasKey(pt => new { pt.ProductId, pt.TagId });

modelBuilder.Entity<ProductTag>()
    .HasOne(pt => pt.Product)
    .WithMany(p => p.ProductTags)
    .HasForeignKey(pt => pt.ProductId);

modelBuilder.Entity<ProductTag>()
    .HasOne(pt => pt.Tag)
    .WithMany(t => t.ProductTags)
    .HasForeignKey(pt => pt.TagId);
```

## 6.7 Best Practices

### 6.7.1 Using Async/Await

```csharp
// ✅ Good - Async
public async Task<IActionResult> Index()
{
    var products = await _context.Products.ToListAsync();
    return View(products);
}

// ❌ Bad - Sync (blocking)
public IActionResult Index()
{
    var products = _context.Products.ToList();
    return View(products);
}
```

### 6.7.2 Dispose DbContext properly

```csharp
// ✅ Good - Using Dependency Injection (auto disposed)
public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }
}

// ❌ Bad - Manual creation
using (var context = new ApplicationDbContext())
{
    // Don't do this in ASP.NET Core
}
```

### 6.7.3 Use AsNoTracking for Read-only

```csharp
// สำหรับ read-only queries
var products = await _context.Products
    .AsNoTracking()
    .ToListAsync();
```

## 6.8 สรุป

ในบทนี้เราได้เรียนรู้:

✅ Entity Framework Core และการติดตั้ง
✅ การสร้าง Models และ DbContext
✅ Migrations และการจัดการฐานข้อมูล
✅ CRUD Operations ด้วย EF Core
✅ การ Query ข้อมูลด้วย LINQ
✅ Relationships (One-to-Many, Many-to-Many)
✅ Best Practices

## 📚 แบบฝึกหัด

1. เพิ่ม Order และ OrderItem models พร้อม relationships
2. สร้างระบบรีวิวสินค้า (Product Reviews)
3. เพิ่ม User model และ Authentication
4. สร้าง Migration สำหรับเพิ่ม field ใหม่
5. Implement Soft Delete (IsDeleted flag)

---

**ก่อนหน้า:** [บทที่ 5 - ASP.NET Core Web Applications](../Chapter05-WebApplications/README.md)
**ต่อไป:** [บทที่ 7 - Web API ด้วย ASP.NET Core](../Chapter07-WebAPI/README.md)
