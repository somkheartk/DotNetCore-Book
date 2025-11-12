using ProductsAPI.Models;

namespace ProductsAPI.Interfaces
{
    /// <summary>
    /// Interface สำหรับ Product Repository
    /// ใช้ Repository Pattern เพื่อแยก business logic ออกจาก data access
    /// </summary>
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(int id, Product product);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Product>> SearchAsync(string keyword);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<bool> ExistsAsync(int id);
    }
}
