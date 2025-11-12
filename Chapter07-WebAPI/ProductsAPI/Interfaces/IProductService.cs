using ProductsAPI.DTOs;

namespace ProductsAPI.Interfaces
{
    /// <summary>
    /// Interface สำหรับ Product Service
    /// ใช้ Service Layer เพื่อจัดการ business logic
    /// </summary>
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto createDto);
        Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto updateDto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductResponseDto>> SearchProductsAsync(string keyword);
        Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(string category);
    }
}
