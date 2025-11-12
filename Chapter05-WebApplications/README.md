# บทที่ 5: ASP.NET Core Web Applications

## 5.1 ความรู้เบื้องต้นเกี่ยวกับ ASP.NET Core

### 5.1.1 ASP.NET Core คืออะไร

ASP.NET Core เป็น Framework สำหรับสร้าง Web Applications และ Web APIs ที่:
- Cross-platform (Windows, Linux, macOS)
- High performance
- Open source
- Modern และ flexible

### 5.1.2 ประเภทของ Web Applications

1. **MVC (Model-View-Controller)**
   - เหมาะสำหรับ Web Applications ขนาดใหญ่
   - แยก concerns อย่างชัดเจน

2. **Razor Pages**
   - เหมาะสำหรับ page-based scenarios
   - Code ง่ายกว่า MVC สำหรับ simple pages

3. **Blazor**
   - เขียน C# แทน JavaScript
   - รันได้ทั้งฝั่ง Server และ Client (WebAssembly)

4. **Web API**
   - สำหรับสร้าง RESTful services
   - ไม่มี UI, return data (JSON/XML)

## 5.2 สร้าง ASP.NET Core MVC Application

### 5.2.1 สร้างโปรเจค

```bash
# สร้าง MVC application
dotnet new mvc -n MyWebApp
cd MyWebApp

# รันแอปพลิเคชัน
dotnet run
```

เปิดเบราว์เซอร์ที่: https://localhost:5001

### 5.2.2 โครงสร้างโปรเจค MVC

```
MyWebApp/
├── Controllers/          # Controllers
│   └── HomeController.cs
├── Models/              # Data models
├── Views/               # Razor views
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       └── _ViewStart.cshtml
├── wwwroot/             # Static files
│   ├── css/
│   ├── js/
│   └── lib/
├── appsettings.json     # Configuration
├── Program.cs           # Entry point
└── MyWebApp.csproj      # Project file
```

## 5.3 MVC Pattern

### 5.3.1 Model

Models แทนข้อมูลและ business logic

สร้างไฟล์ `Models/Product.cs`:

```csharp
namespace MyWebApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = "/images/no-image.png";
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsAvailable => Stock > 0;

        public string GetPriceFormatted()
        {
            return Price.ToString("N2") + " บาท";
        }
    }
}
```

สร้าง ViewModel `Models/ProductViewModel.cs`:

```csharp
namespace MyWebApp.Models
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string SelectedCategory { get; set; } = "All";
        public List<string> Categories { get; set; } = new();
        public int TotalProducts { get; set; }
    }

    public class ProductDetailViewModel
    {
        public Product Product { get; set; } = new();
        public List<Product> RelatedProducts { get; set; } = new();
    }
}
```

### 5.3.2 Controller

Controllers จัดการ requests และ responses

สร้างไฟล์ `Controllers/ProductController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class ProductController : Controller
    {
        // Mock data - ในโปรเจคจริงจะดึงจากฐานข้อมูล
        private static List<Product> _products = new()
        {
            new Product 
            { 
                Id = 1, 
                Name = "โน้ตบุ๊ค Dell XPS 13", 
                Description = "โน้ตบุ๊คพกพาสเปกสูง",
                Price = 45000, 
                Category = "คอมพิวเตอร์",
                Stock = 10 
            },
            new Product 
            { 
                Id = 2, 
                Name = "iPhone 15 Pro", 
                Description = "สมาร์ทโฟนล่าสุดจาก Apple",
                Price = 38900, 
                Category = "โทรศัพท์",
                Stock = 15 
            },
            new Product 
            { 
                Id = 3, 
                Name = "iPad Air", 
                Description = "แท็บเล็ตสำหรับทำงานและความบันเทิง",
                Price = 22900, 
                Category = "แท็บเล็ต",
                Stock = 8 
            }
        };

        // GET: /Product
        public IActionResult Index(string searchTerm, string category)
        {
            var viewModel = new ProductListViewModel
            {
                SearchTerm = searchTerm ?? string.Empty,
                SelectedCategory = category ?? "All"
            };

            // กรองสินค้า
            var products = _products.AsEnumerable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => 
                    p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (category != "All" && !string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            viewModel.Products = products.ToList();
            viewModel.TotalProducts = viewModel.Products.Count;
            viewModel.Categories = _products.Select(p => p.Category).Distinct().ToList();

            return View(viewModel);
        }

        // GET: /Product/Details/1
        public IActionResult Details(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                RelatedProducts = _products
                    .Where(p => p.Category == product.Category && p.Id != product.Id)
                    .Take(3)
                    .ToList()
            };

            return View(viewModel);
        }

        // GET: /Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                product.Id = _products.Max(p => p.Id) + 1;
                _products.Add(product);
                
                TempData["SuccessMessage"] = "เพิ่มสินค้าเรียบร้อยแล้ว";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // GET: /Product/Edit/1
        public IActionResult Edit(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: /Product/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var existingProduct = _products.FirstOrDefault(p => p.Id == id);
                if (existingProduct != null)
                {
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.Category = product.Category;
                    existingProduct.Stock = product.Stock;

                    TempData["SuccessMessage"] = "แก้ไขสินค้าเรียบร้อยแล้ว";
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(product);
        }

        // POST: /Product/Delete/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            
            if (product != null)
            {
                _products.Remove(product);
                TempData["SuccessMessage"] = "ลบสินค้าเรียบร้อยแล้ว";
            }

            return RedirectToAction(nameof(Index));
        }

        // API endpoint สำหรับ AJAX
        [HttpGet]
        public JsonResult Search(string term)
        {
            var products = _products
                .Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(p => new { p.Id, p.Name, p.Price })
                .ToList();

            return Json(products);
        }
    }
}
```

### 5.3.3 View

Views แสดงผล UI ด้วย Razor syntax

สร้างไฟล์ `Views/Product/Index.cshtml`:

```html
@model MyWebApp.Models.ProductListViewModel

@{
    ViewData["Title"] = "รายการสินค้า";
}

<div class="container mt-4">
    <div class="row mb-4">
        <div class="col-md-12">
            <h1 class="display-4">
                <i class="bi bi-shop"></i> รายการสินค้า
            </h1>
        </div>
    </div>

    <!-- Search and Filter -->
    <div class="row mb-4">
        <div class="col-md-12">
            <form asp-action="Index" method="get" class="row g-3">
                <div class="col-md-6">
                    <div class="input-group">
                        <input type="text" 
                               class="form-control" 
                               name="searchTerm" 
                               value="@Model.SearchTerm"
                               placeholder="ค้นหาสินค้า...">
                        <button class="btn btn-primary" type="submit">
                            <i class="bi bi-search"></i> ค้นหา
                        </button>
                    </div>
                </div>
                <div class="col-md-4">
                    <select class="form-select" name="category" onchange="this.form.submit()">
                        <option value="All">หมวดหมู่ทั้งหมด</option>
                        @foreach (var cat in Model.Categories)
                        {
                            <option value="@cat" selected="@(cat == Model.SelectedCategory)">
                                @cat
                            </option>
                        }
                    </select>
                </div>
                <div class="col-md-2">
                    <a asp-action="Create" class="btn btn-success w-100">
                        <i class="bi bi-plus-circle"></i> เพิ่มสินค้า
                    </a>
                </div>
            </form>
        </div>
    </div>

    <!-- Success Message -->
    @if (TempData["SuccessMessage"] != null)
    {
        <div class="alert alert-success alert-dismissible fade show" role="alert">
            @TempData["SuccessMessage"]
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    }

    <!-- Product Count -->
    <div class="row mb-3">
        <div class="col-md-12">
            <p class="text-muted">พบสินค้า @Model.TotalProducts รายการ</p>
        </div>
    </div>

    <!-- Products Grid -->
    <div class="row">
        @foreach (var product in Model.Products)
        {
            <div class="col-md-4 mb-4">
                <div class="card h-100 shadow-sm">
                    <img src="@product.ImageUrl" 
                         class="card-img-top" 
                         alt="@product.Name"
                         style="height: 200px; object-fit: cover;">
                    <div class="card-body">
                        <h5 class="card-title">@product.Name</h5>
                        <p class="card-text text-muted">@product.Description</p>
                        <p class="card-text">
                            <span class="badge bg-secondary">@product.Category</span>
                        </p>
                        <div class="d-flex justify-content-between align-items-center">
                            <h4 class="text-primary mb-0">@product.GetPriceFormatted()</h4>
                            @if (product.IsAvailable)
                            {
                                <span class="badge bg-success">มีสินค้า (@product.Stock)</span>
                            }
                            else
                            {
                                <span class="badge bg-danger">สินค้าหมด</span>
                            }
                        </div>
                    </div>
                    <div class="card-footer bg-transparent">
                        <div class="btn-group w-100" role="group">
                            <a asp-action="Details" 
                               asp-route-id="@product.Id" 
                               class="btn btn-outline-primary">
                                <i class="bi bi-eye"></i> ดูรายละเอียด
                            </a>
                            <a asp-action="Edit" 
                               asp-route-id="@product.Id" 
                               class="btn btn-outline-warning">
                                <i class="bi bi-pencil"></i> แก้ไข
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        }
    </div>

    @if (Model.Products.Count == 0)
    {
        <div class="row">
            <div class="col-md-12 text-center py-5">
                <i class="bi bi-inbox" style="font-size: 4rem; color: #ccc;"></i>
                <h3 class="text-muted mt-3">ไม่พบสินค้า</h3>
                <p>ลองค้นหาด้วยคำอื่นหรือเพิ่มสินค้าใหม่</p>
            </div>
        </div>
    }
</div>
```

สร้างไฟล์ `Views/Product/Details.cshtml`:

```html
@model MyWebApp.Models.ProductDetailViewModel

@{
    ViewData["Title"] = Model.Product.Name;
}

<div class="container mt-4">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a asp-controller="Home" asp-action="Index">หน้าหลัก</a></li>
            <li class="breadcrumb-item"><a asp-action="Index">สินค้า</a></li>
            <li class="breadcrumb-item active">@Model.Product.Name</li>
        </ol>
    </nav>

    <!-- Product Detail -->
    <div class="row">
        <div class="col-md-6">
            <img src="@Model.Product.ImageUrl" 
                 class="img-fluid rounded shadow" 
                 alt="@Model.Product.Name">
        </div>
        <div class="col-md-6">
            <h1 class="display-5">@Model.Product.Name</h1>
            <p class="lead">@Model.Product.Description</p>
            
            <hr>

            <div class="mb-3">
                <span class="badge bg-primary fs-6">@Model.Product.Category</span>
            </div>

            <h2 class="text-primary mb-3">@Model.Product.GetPriceFormatted()</h2>

            <div class="mb-3">
                @if (Model.Product.IsAvailable)
                {
                    <span class="badge bg-success fs-6">
                        <i class="bi bi-check-circle"></i> มีสินค้า (เหลือ @Model.Product.Stock ชิ้น)
                    </span>
                }
                else
                {
                    <span class="badge bg-danger fs-6">
                        <i class="bi bi-x-circle"></i> สินค้าหมด
                    </span>
                }
            </div>

            <div class="d-grid gap-2 d-md-flex">
                <button type="button" class="btn btn-primary btn-lg" disabled="@(!Model.Product.IsAvailable)">
                    <i class="bi bi-cart-plus"></i> เพิ่มลงตะกร้า
                </button>
                <a asp-action="Edit" 
                   asp-route-id="@Model.Product.Id" 
                   class="btn btn-warning btn-lg">
                    <i class="bi bi-pencil"></i> แก้ไข
                </a>
                <button type="button" 
                        class="btn btn-danger btn-lg"
                        onclick="confirmDelete(@Model.Product.Id)">
                    <i class="bi bi-trash"></i> ลบ
                </button>
            </div>

            <div class="mt-4">
                <small class="text-muted">
                    เพิ่มเมื่อ: @Model.Product.CreatedDate.ToString("dd MMMM yyyy HH:mm")
                </small>
            </div>
        </div>
    </div>

    <!-- Related Products -->
    @if (Model.RelatedProducts.Any())
    {
        <div class="row mt-5">
            <div class="col-md-12">
                <h3>สินค้าที่เกี่ยวข้อง</h3>
                <hr>
            </div>
        </div>

        <div class="row">
            @foreach (var product in Model.RelatedProducts)
            {
                <div class="col-md-4 mb-4">
                    <div class="card">
                        <img src="@product.ImageUrl" class="card-img-top" alt="@product.Name">
                        <div class="card-body">
                            <h5 class="card-title">@product.Name</h5>
                            <p class="card-text text-primary fw-bold">@product.GetPriceFormatted()</p>
                            <a asp-action="Details" 
                               asp-route-id="@product.Id" 
                               class="btn btn-outline-primary btn-sm">
                                ดูรายละเอียด
                            </a>
                        </div>
                    </div>
                </div>
            }
        </div>
    }
</div>

<!-- Delete Confirmation Form -->
<form id="deleteForm" asp-action="Delete" method="post" style="display: none;">
    <input type="hidden" name="id" id="deleteId">
    @Html.AntiForgeryToken()
</form>

@section Scripts {
    <script>
        function confirmDelete(id) {
            if (confirm('คุณแน่ใจหรือไม่ว่าต้องการลบสินค้านี้?')) {
                document.getElementById('deleteId').value = id;
                document.getElementById('deleteForm').submit();
            }
        }
    </script>
}
```

สร้างไฟล์ `Views/Product/Create.cshtml`:

```html
@model MyWebApp.Models.Product

@{
    ViewData["Title"] = "เพิ่มสินค้าใหม่";
}

<div class="container mt-4">
    <div class="row">
        <div class="col-md-8 offset-md-2">
            <h1 class="mb-4">
                <i class="bi bi-plus-circle"></i> เพิ่มสินค้าใหม่
            </h1>

            <div class="card">
                <div class="card-body">
                    <form asp-action="Create" method="post">
                        <div asp-validation-summary="ModelOnly" class="text-danger"></div>

                        <div class="mb-3">
                            <label asp-for="Name" class="form-label"></label>
                            <input asp-for="Name" class="form-control" />
                            <span asp-validation-for="Name" class="text-danger"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Description" class="form-label"></label>
                            <textarea asp-for="Description" class="form-control" rows="3"></textarea>
                            <span asp-validation-for="Description" class="text-danger"></span>
                        </div>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="mb-3">
                                    <label asp-for="Price" class="form-label"></label>
                                    <div class="input-group">
                                        <input asp-for="Price" class="form-control" type="number" step="0.01" />
                                        <span class="input-group-text">บาท</span>
                                    </div>
                                    <span asp-validation-for="Price" class="text-danger"></span>
                                </div>
                            </div>

                            <div class="col-md-6">
                                <div class="mb-3">
                                    <label asp-for="Stock" class="form-label"></label>
                                    <input asp-for="Stock" class="form-control" type="number" />
                                    <span asp-validation-for="Stock" class="text-danger"></span>
                                </div>
                            </div>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Category" class="form-label"></label>
                            <select asp-for="Category" class="form-select">
                                <option value="">-- เลือกหมวดหมู่ --</option>
                                <option value="คอมพิวเตอร์">คอมพิวเตอร์</option>
                                <option value="โทรศัพท์">โทรศัพท์</option>
                                <option value="แท็บเล็ต">แท็บเล็ต</option>
                                <option value="อุปกรณ์เสริม">อุปกรณ์เสริม</option>
                            </select>
                            <span asp-validation-for="Category" class="text-danger"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="ImageUrl" class="form-label"></label>
                            <input asp-for="ImageUrl" class="form-control" />
                            <span asp-validation-for="ImageUrl" class="text-danger"></span>
                            <small class="text-muted">ใส่ URL ของรูปภาพ</small>
                        </div>

                        <hr>

                        <div class="d-grid gap-2 d-md-flex justify-content-md-end">
                            <a asp-action="Index" class="btn btn-secondary">
                                <i class="bi bi-x-circle"></i> ยกเลิก
                            </a>
                            <button type="submit" class="btn btn-primary">
                                <i class="bi bi-save"></i> บันทึก
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

## 5.4 Layout และ Shared Views

### 5.4.1 _Layout.cshtml

แก้ไขไฟล์ `Views/Shared/_Layout.cshtml`:

```html
<!DOCTYPE html>
<html lang="th">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - My Web App</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.0/font/bootstrap-icons.css">
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
</head>
<body>
    <header>
        <nav class="navbar navbar-expand-sm navbar-dark bg-primary mb-3">
            <div class="container">
                <a class="navbar-brand" asp-controller="Home" asp-action="Index">
                    <i class="bi bi-shop"></i> My Shop
                </a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Home" asp-action="Index">หน้าหลัก</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Product" asp-action="Index">สินค้า</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Home" asp-action="About">เกี่ยวกับเรา</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Home" asp-action="Contact">ติดต่อเรา</a>
                        </li>
                    </ul>
                    <ul class="navbar-nav">
                        <li class="nav-item">
                            <a class="nav-link" href="#">
                                <i class="bi bi-cart"></i> ตะกร้า (0)
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#">
                                <i class="bi bi-person"></i> เข้าสู่ระบบ
                            </a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>

    <main role="main" class="pb-3">
        @RenderBody()
    </main>

    <footer class="border-top footer text-muted bg-light mt-5">
        <div class="container py-3">
            <div class="row">
                <div class="col-md-4">
                    <h5>My Shop</h5>
                    <p>ร้านค้าออนไลน์คุณภาพ</p>
                </div>
                <div class="col-md-4">
                    <h5>ลิงก์ด่วน</h5>
                    <ul class="list-unstyled">
                        <li><a asp-controller="Home" asp-action="Index">หน้าหลัก</a></li>
                        <li><a asp-controller="Product" asp-action="Index">สินค้า</a></li>
                        <li><a asp-controller="Home" asp-action="Privacy">นโยบายความเป็นส่วนตัว</a></li>
                    </ul>
                </div>
                <div class="col-md-4">
                    <h5>ติดตามเรา</h5>
                    <a href="#" class="me-2"><i class="bi bi-facebook"></i></a>
                    <a href="#" class="me-2"><i class="bi bi-twitter"></i></a>
                    <a href="#" class="me-2"><i class="bi bi-instagram"></i></a>
                    <a href="#"><i class="bi bi-youtube"></i></a>
                </div>
            </div>
            <hr>
            <div class="text-center">
                &copy; 2024 - My Web App - <a asp-controller="Home" asp-action="Privacy">Privacy</a>
            </div>
        </div>
    </footer>

    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

## 5.5 Routing

### 5.5.1 Convention-based Routing

แก้ไขใน `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Custom routes
app.MapControllerRoute(
    name: "products",
    pattern: "products/{category?}",
    defaults: new { controller = "Product", action = "Index" });

app.Run();
```

### 5.5.2 Attribute Routing

```csharp
[Route("api/[controller]")]
public class ProductApiController : Controller
{
    [HttpGet]
    [Route("")]
    public IActionResult GetAll()
    {
        // GET: /api/productapi
        return Ok(products);
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult GetById(int id)
    {
        // GET: /api/productapi/1
        return Ok(product);
    }

    [HttpPost]
    [Route("")]
    public IActionResult Create([FromBody] Product product)
    {
        // POST: /api/productapi
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
}
```

## 5.6 สรุป

ในบทนี้เราได้เรียนรู้:

✅ ความรู้เบื้องต้นเกี่ยวกับ ASP.NET Core
✅ MVC Pattern (Model-View-Controller)
✅ การสร้าง Controllers และ Actions
✅ การสร้าง Views ด้วย Razor
✅ Layout และ Shared Views
✅ Routing (Convention-based และ Attribute Routing)

## 📚 แบบฝึกหัด

1. เพิ่มฟีเจอร์ pagination ในหน้ารายการสินค้า
2. สร้างหน้าตะกร้าสินค้า (Shopping Cart)
3. เพิ่มระบบ Login/Register
4. สร้าง Dashboard สำหรับ Admin
5. เพิ่มการ Upload รูปภาพสินค้า

---

**ก่อนหน้า:** [บทที่ 4 - สร้าง Console Application แรก](../Chapter04-ConsoleApplication/README.md)
**ต่อไป:** [บทที่ 6 - การทำงานกับฐานข้อมูล](../Chapter06-Database/README.md)
