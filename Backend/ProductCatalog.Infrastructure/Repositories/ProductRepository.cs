using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsWithDetailsAsync(int page, int pageSize, int? categoryId, decimal? minPrice, decimal? maxPrice, string? nameFilter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryID == categoryId);

            if (minPrice.HasValue)
                query = query.Where(p => p.UnitPrice >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.UnitPrice <= maxPrice);

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(p => p.ProductName.ToLower().Contains(nameFilter.ToLower()));

            return await query
                .OrderBy(p => p.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, string? nameFilter)
        {
            var query = _context.Products.AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryID == categoryId);

            if (minPrice.HasValue)
                query = query.Where(p => p.UnitPrice >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.UnitPrice <= maxPrice);

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(p => p.ProductName.ToLower().Contains(nameFilter.ToLower()));

            return await query.CountAsync();
        }

        public async Task<Product?> GetProductByIdWithDetailsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductID == id);
        }
    }
}
