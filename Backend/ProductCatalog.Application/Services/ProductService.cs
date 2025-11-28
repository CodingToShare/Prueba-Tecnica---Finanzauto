using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;
using System.Diagnostics;

namespace ProductCatalog.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private static readonly Random _random = new Random();

        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ISupplierRepository supplierRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                SupplierID = dto.SupplierID,
                CategoryID = dto.CategoryID,
                QuantityPerUnit = dto.QuantityPerUnit,
                UnitPrice = dto.UnitPrice,
                UnitsInStock = dto.UnitsInStock,
                UnitsOnOrder = dto.UnitsOnOrder,
                ReorderLevel = dto.ReorderLevel,
                Discontinued = dto.Discontinued
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<PagedResultDto<ProductDto>> GetProductsAsync(
            int page,
            int pageSize,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? search)
        {
            var products = await _productRepository.GetProductsWithDetailsAsync(
                page, pageSize, categoryId, minPrice, maxPrice, search);

            var totalCount = await _productRepository.CountAsync(
                categoryId, minPrice, maxPrice, search);

            var productDtos = products.Select(MapToDto).ToList();

            return new PagedResultDto<ProductDto>
            {
                Items = productDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ProductDetailDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductByIdWithDetailsAsync(id);
            if (product == null) return null;

            return MapToDetailDto(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            product.ProductName = dto.ProductName;
            product.SupplierID = dto.SupplierID;
            product.CategoryID = dto.CategoryID;
            product.QuantityPerUnit = dto.QuantityPerUnit;
            product.UnitPrice = dto.UnitPrice;
            product.UnitsInStock = dto.UnitsInStock;
            product.UnitsOnOrder = dto.UnitsOnOrder;
            product.ReorderLevel = dto.ReorderLevel;
            product.Discontinued = dto.Discontinued;

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            // Soft delete using Discontinued flag
            product.Discontinued = true;
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();

            return true;
        }

        public async Task<BulkInsertResultDto> BulkGenerateProductsAsync(int count)
        {
            var stopwatch = Stopwatch.StartNew();

            // Get available categories and suppliers
            var categories = (await _categoryRepository.GetAllAsync()).ToList();
            var suppliers = (await _supplierRepository.GetAllAsync()).ToList();

            if (categories.Count == 0 || suppliers.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cannot generate products without existing categories and suppliers. Please create them first.");
            }

            // Generate products in batches of 10,000 for optimal performance
            const int batchSize = 10000;
            int totalInserted = 0;

            for (int i = 0; i < count; i += batchSize)
            {
                int currentBatchSize = Math.Min(batchSize, count - i);
                var products = new List<Product>(currentBatchSize);

                for (int j = 0; j < currentBatchSize; j++)
                {
                    products.Add(GenerateRandomProduct(categories, suppliers));
                }

                await _productRepository.AddRangeAsync(products);
                await _productRepository.SaveChangesAsync();

                totalInserted += currentBatchSize;
            }

            stopwatch.Stop();

            return new BulkInsertResultDto
            {
                ProductsCreated = totalInserted,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                Message = $"Successfully created {totalInserted} products in {stopwatch.ElapsedMilliseconds}ms"
            };
        }

        private Product GenerateRandomProduct(List<Category> categories, List<Supplier> suppliers)
        {
            var category = categories[_random.Next(categories.Count)];
            var supplier = suppliers[_random.Next(suppliers.Count)];

            return new Product
            {
                ProductName = GenerateRandomProductName(category.CategoryName),
                SupplierID = supplier.SupplierID,
                CategoryID = category.CategoryID,
                QuantityPerUnit = GenerateQuantityPerUnit(),
                UnitPrice = GenerateRandomPrice(),
                UnitsInStock = (short)_random.Next(0, 500),
                UnitsOnOrder = (short)_random.Next(0, 100),
                ReorderLevel = (short)_random.Next(5, 50),
                Discontinued = _random.Next(100) < 10 // 10% discontinued
            };
        }

        private static string GenerateRandomProductName(string categoryName)
        {
            var adjectives = new[] { "Premium", "Standard", "Professional", "Advanced", "Basic", "Deluxe", "Ultimate", "Express" };
            var prefixes = new[] { "Super", "Mega", "Ultra", "Pro", "Max", "Plus", "Turbo", "Elite" };

            var adjective = adjectives[_random.Next(adjectives.Length)];
            var prefix = _random.Next(2) == 0 ? prefixes[_random.Next(prefixes.Length)] + " " : "";
            var suffix = _random.Next(1000, 9999);

            return $"{prefix}{adjective} {categoryName} {suffix}";
        }

        private static string GenerateQuantityPerUnit()
        {
            var quantities = new[] { "1 box", "10 units", "12 items", "24 pack", "6 bottles", "1 kg", "500 g", "1 L", "100 pieces" };
            return quantities[_random.Next(quantities.Length)];
        }

        private static decimal GenerateRandomPrice()
        {
            return Math.Round((decimal)(_random.NextDouble() * 990 + 10), 2); // Between $10 and $1000
        }

        private static ProductDto MapToDto(Product p)
        {
            return new ProductDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                SupplierID = p.SupplierID,
                CategoryID = p.CategoryID,
                QuantityPerUnit = p.QuantityPerUnit,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock,
                UnitsOnOrder = p.UnitsOnOrder,
                ReorderLevel = p.ReorderLevel,
                Discontinued = p.Discontinued,
                CategoryName = p.Category?.CategoryName,
                SupplierName = p.Supplier?.CompanyName
            };
        }

        private static ProductDetailDto MapToDetailDto(Product p)
        {
            return new ProductDetailDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                SupplierID = p.SupplierID,
                CategoryID = p.CategoryID,
                QuantityPerUnit = p.QuantityPerUnit,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock,
                UnitsOnOrder = p.UnitsOnOrder,
                ReorderLevel = p.ReorderLevel,
                Discontinued = p.Discontinued,
                Category = p.Category != null ? new CategoryDto
                {
                    CategoryID = p.Category.CategoryID,
                    CategoryName = p.Category.CategoryName,
                    Description = p.Category.Description,
                    Picture = p.Category.Picture
                } : null,
                Supplier = p.Supplier != null ? new SupplierDto
                {
                    SupplierID = p.Supplier.SupplierID,
                    CompanyName = p.Supplier.CompanyName,
                    ContactName = p.Supplier.ContactName,
                    City = p.Supplier.City,
                    Country = p.Supplier.Country,
                    Phone = p.Supplier.Phone
                } : null
            };
        }
    }
}
