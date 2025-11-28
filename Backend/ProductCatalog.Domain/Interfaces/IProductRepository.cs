using ProductCatalog.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductCatalog.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithDetailsAsync(int page, int pageSize, int? categoryId, decimal? minPrice, decimal? maxPrice, string? nameFilter);
        Task<Product?> GetProductByIdWithDetailsAsync(int id);
        Task<int> CountAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, string? nameFilter);
    }
}
