using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<PagedResultDto<ProductDto>> GetProductsAsync(int page, int pageSize, int? categoryId, decimal? minPrice, decimal? maxPrice, string? search);
        Task<ProductDetailDto?> GetProductByIdAsync(int id);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int id);
        Task<BulkInsertResultDto> BulkGenerateProductsAsync(int count);
    }
}
