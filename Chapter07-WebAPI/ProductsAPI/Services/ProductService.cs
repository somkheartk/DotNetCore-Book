using ProductsAPI.DTOs;
using ProductsAPI.Interfaces;
using ProductsAPI.Models;

namespace ProductsAPI.Services
{
    /// <summary>
    /// Service Layer สำหรับจัดการ business logic ของสินค้า
    /// แยก business logic ออกจาก Controller ทำให้โค้ดดูแลรักษาง่ายและทดสอบได้ง่ายขึ้น
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            _logger.LogInformation("กำลังดึงข้อมูลสินค้าทั้งหมด");
            
            var products = await _repository.GetAllAsync();
            return products.Select(MapToResponseDto);
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("กำลังดึงข้อมูลสินค้า ID: {ProductId}", id);
            
            var product = await _repository.GetByIdAsync(id);
            return product != null ? MapToResponseDto(product) : null;
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto createDto)
        {
            _logger.LogInformation("กำลังสร้างสินค้าใหม่: {ProductName}", createDto.Name);
            
            // แปลง DTO เป็น Entity
            var product = new Product
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Price = createDto.Price,
                Stock = createDto.Stock,
                Category = createDto.Category,
                IsActive = createDto.IsActive
            };

            var createdProduct = await _repository.CreateAsync(product);
            
            _logger.LogInformation("สร้างสินค้าสำเร็จ ID: {ProductId}", createdProduct.Id);
            
            return MapToResponseDto(createdProduct);
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto updateDto)
        {
            _logger.LogInformation("กำลังอัพเดทสินค้า ID: {ProductId}", id);
            
            // ตรวจสอบว่าสินค้ามีอยู่หรือไม่
            var exists = await _repository.ExistsAsync(id);
            if (!exists)
            {
                _logger.LogWarning("ไม่พบสินค้า ID: {ProductId}", id);
                return null;
            }

            // แปลง DTO เป็น Entity
            var product = new Product
            {
                Name = updateDto.Name,
                Description = updateDto.Description,
                Price = updateDto.Price,
                Stock = updateDto.Stock,
                Category = updateDto.Category,
                IsActive = updateDto.IsActive
            };

            var updatedProduct = await _repository.UpdateAsync(id, product);
            
            if (updatedProduct != null)
            {
                _logger.LogInformation("อัพเดทสินค้าสำเร็จ ID: {ProductId}", id);
            }

            return updatedProduct != null ? MapToResponseDto(updatedProduct) : null;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogInformation("กำลังลบสินค้า ID: {ProductId}", id);
            
            var result = await _repository.DeleteAsync(id);
            
            if (result)
            {
                _logger.LogInformation("ลบสินค้าสำเร็จ ID: {ProductId}", id);
            }
            else
            {
                _logger.LogWarning("ไม่พบสินค้าที่ต้องการลบ ID: {ProductId}", id);
            }

            return result;
        }

        public async Task<IEnumerable<ProductResponseDto>> SearchProductsAsync(string keyword)
        {
            _logger.LogInformation("กำลังค้นหาสินค้าด้วยคำค้นหา: {Keyword}", keyword);
            
            var products = await _repository.SearchAsync(keyword);
            return products.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(string category)
        {
            _logger.LogInformation("กำลังดึงสินค้าในหมวดหมู่: {Category}", category);
            
            var products = await _repository.GetByCategoryAsync(category);
            return products.Select(MapToResponseDto);
        }

        // Helper method สำหรับแปลง Entity เป็น DTO
        private static ProductResponseDto MapToResponseDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Category = product.Category,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
