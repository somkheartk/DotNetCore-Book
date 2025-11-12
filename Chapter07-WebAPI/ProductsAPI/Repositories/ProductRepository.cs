using ProductsAPI.Interfaces;
using ProductsAPI.Models;

namespace ProductsAPI.Repositories
{
    /// <summary>
    /// In-Memory Repository สำหรับการจัดการข้อมูลสินค้า
    /// ในโปรเจคจริง ควรใช้ Entity Framework Core กับฐานข้อมูล
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        // ใช้ static list เพื่อเก็บข้อมูลชั่วคระหว่างรันแอป (In-Memory)
        private static readonly List<Product> _products = new()
        {
            new Product
            {
                Id = 1,
                Name = "โน้ตบุ๊ค Dell XPS 13",
                Description = "โน้ตบุ๊คสำหรับการทำงาน พร้อม Intel Core i7",
                Price = 45000,
                Stock = 15,
                Category = "Electronics",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new Product
            {
                Id = 2,
                Name = "เมาส์ไร้สาย Logitech MX Master 3",
                Description = "เมาส์สำหรับมืออาชีพ ดีไซน์สวย",
                Price = 3500,
                Stock = 50,
                Category = "Electronics",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new Product
            {
                Id = 3,
                Name = "คีย์บอร์ดเกมมิ่ง Razer",
                Description = "คีย์บอร์ดเกมมิ่ง RGB สำหรับเกมเมอร์",
                Price = 4500,
                Stock = 30,
                Category = "Electronics",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new Product
            {
                Id = 4,
                Name = "หนังสือ Clean Code",
                Description = "หนังสือสอนเขียนโค้ดที่สะอาดและดูแลรักษาง่าย",
                Price = 850,
                Stock = 100,
                Category = "Books",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Product
            {
                Id = 5,
                Name = "หูฟัง Sony WH-1000XM5",
                Description = "หูฟังตัดเสียงรบกวนระดับพรีเมียม",
                Price = 12000,
                Stock = 0,
                Category = "Electronics",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        private static int _nextId = 6;

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            // เรียงตามวันที่สร้างล่าสุด
            var products = _products
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
            return Task.FromResult<IEnumerable<Product>>(products);
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
        }

        public Task<Product> CreateAsync(Product product)
        {
            product.Id = _nextId++;
            product.CreatedAt = DateTime.UtcNow;
            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task<Product?> UpdateAsync(int id, Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return Task.FromResult<Product?>(null);
            }

            // อัพเดทข้อมูล
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.Category = product.Category;
            existingProduct.IsActive = product.IsActive;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            return Task.FromResult<Product?>(existingProduct);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return Task.FromResult(false);
            }

            _products.Remove(product);
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Product>> SearchAsync(string keyword)
        {
            var results = _products
                .Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                           (p.Description != null && p.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return Task.FromResult<IEnumerable<Product>>(results);
        }

        public Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            var results = _products
                .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return Task.FromResult<IEnumerable<Product>>(results);
        }

        public Task<bool> ExistsAsync(int id)
        {
            var exists = _products.Any(p => p.Id == id);
            return Task.FromResult(exists);
        }
    }
}
