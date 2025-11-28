using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Application.DTOs
{
    public class ProductDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? SupplierID { get; set; }
        public int? CategoryID { get; set; }
        public string? QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public string? CategoryName { get; set; }
        public string? SupplierName { get; set; }
    }

    public class ProductDetailDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int? SupplierID { get; set; }
        public int? CategoryID { get; set; }
        public string? QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public CategoryDto? Category { get; set; }
        public SupplierDto? Supplier { get; set; }
    }

    public class CreateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 40 characters")]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Supplier ID must be greater than 0")]
        public int? SupplierID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        public int? CategoryID { get; set; }

        [StringLength(50, ErrorMessage = "Quantity per unit must not exceed 50 characters")]
        public string? QuantityPerUnit { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Unit price must be between 0 and 999,999.99")]
        public decimal? UnitPrice { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Units in stock must be non-negative")]
        public short? UnitsInStock { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Units on order must be non-negative")]
        public short? UnitsOnOrder { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Reorder level must be non-negative")]
        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; }
    }

    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 40 characters")]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Supplier ID must be greater than 0")]
        public int? SupplierID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
        public int? CategoryID { get; set; }

        [StringLength(50, ErrorMessage = "Quantity per unit must not exceed 50 characters")]
        public string? QuantityPerUnit { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Unit price must be between 0 and 999,999.99")]
        public decimal? UnitPrice { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Units in stock must be non-negative")]
        public short? UnitsInStock { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Units on order must be non-negative")]
        public short? UnitsOnOrder { get; set; }

        [Range(0, short.MaxValue, ErrorMessage = "Reorder level must be non-negative")]
        public short? ReorderLevel { get; set; }

        public bool Discontinued { get; set; }
    }

    public class BulkGenerateProductsDto
    {
        [Range(1, 100000, ErrorMessage = "Count must be between 1 and 100,000")]
        public int Count { get; set; } = 1000;
    }

    public class BulkInsertResultDto
    {
        public int ProductsCreated { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
