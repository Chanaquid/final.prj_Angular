using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<IReadOnlyList<Product>> GetProductsAsync(string sort, int pageSize, int pageIndex);
        Task AddProductAsync(Product product);
        void UpdateProduct(Product product);
        void DeleteProductAsync(Product product);
        Task<int> SaveChangesAsync();

    
    }
}