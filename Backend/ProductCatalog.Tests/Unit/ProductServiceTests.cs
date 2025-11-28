using Moq;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Services;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;
using Xunit;

namespace ProductCatalog.Tests.Unit
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<ISupplierRepository> _supplierRepositoryMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _supplierRepositoryMock = new Mock<ISupplierRepository>();

            _productService = new ProductService(
                _productRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _supplierRepositoryMock.Object
            );
        }

        [Fact]
        public async Task CreateProductAsync_WithValidData_ReturnsProductDto()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                ProductName = "Test Product",
                SupplierID = 1,
                CategoryID = 1,
                UnitPrice = 99.99m,
                UnitsInStock = 100
            };

            _productRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);
            _productRepositoryMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Product", result.ProductName);
            Assert.Equal(99.99m, result.UnitPrice);
            Assert.Equal((short)100, result.UnitsInStock);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsPagedResult()
        {
            // Arrange
            var product1 = new Product
            {
                ProductID = 1,
                ProductName = "Product 1",
                UnitPrice = 10m
            };
            var product2 = new Product
            {
                ProductID = 2,
                ProductName = "Product 2",
                UnitPrice = 20m
            };
            var products = new List<Product> { product1, product2 };

            _productRepositoryMock.Setup(r => r.GetProductsWithDetailsAsync(1, 10, null, null, null, null))
                .ReturnsAsync(products);
            _productRepositoryMock.Setup(r => r.CountAsync(null, null, null, null))
                .ReturnsAsync(2);

            // Act
            var result = await _productService.GetProductsAsync(1, 10, null, null, null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithExistingId_ReturnsProductDetail()
        {
            // Arrange
            var product = new Product
            {
                ProductID = 1,
                ProductName = "Test Product",
                Category = new Category { CategoryID = 1, CategoryName = "Test Category" },
                Supplier = new Supplier { SupplierID = 1, CompanyName = "Test Supplier" }
            };

            _productRepositoryMock.Setup(r => r.GetProductByIdWithDetailsAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ProductID);
            Assert.Equal("Test Product", result.ProductName);
            Assert.NotNull(result.Category);
            Assert.Equal("Test Category", result.Category.CategoryName);
            Assert.NotNull(result.Supplier);
            Assert.Equal("Test Supplier", result.Supplier.CompanyName);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            _productRepositoryMock.Setup(r => r.GetProductByIdWithDetailsAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.GetProductByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProductAsync_WithValidData_ReturnsUpdatedProduct()
        {
            // Arrange
            var existingProduct = new Product
            {
                ProductID = 1,
                ProductName = "Old Name",
                UnitPrice = 10
            };

            var updateDto = new UpdateProductDto
            {
                ProductName = "New Name",
                UnitPrice = 20
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingProduct);
            _productRepositoryMock.Setup(r => r.Update(It.IsAny<Product>()));
            _productRepositoryMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.UpdateProductAsync(1, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Name", result.ProductName);
            Assert.Equal(20, result.UnitPrice);
        }

        [Fact]
        public async Task UpdateProductAsync_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            _productRepositoryMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            var updateDto = new UpdateProductDto { ProductName = "Test" };

            // Act
            var result = await _productService.UpdateProductAsync(999, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteProductAsync_WithExistingId_SetsDiscontinuedToTrue()
        {
            // Arrange
            var product = new Product
            {
                ProductID = 1,
                ProductName = "Test Product",
                Discontinued = false
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product);
            _productRepositoryMock.Setup(r => r.Update(It.IsAny<Product>()));
            _productRepositoryMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.DeleteProductAsync(1);

            // Assert
            Assert.True(result);
            Assert.True(product.Discontinued);
            _productRepositoryMock.Verify(r => r.Update(It.Is<Product>(p => p.Discontinued == true)), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_WithNonExistingId_ReturnsFalse()
        {
            // Arrange
            _productRepositoryMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.DeleteProductAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task BulkGenerateProductsAsync_WithValidCount_CreatesProducts()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryID = 1, CategoryName = "Electronics" },
                new Category { CategoryID = 2, CategoryName = "Books" }
            };

            var suppliers = new List<Supplier>
            {
                new Supplier { SupplierID = 1, CompanyName = "Supplier A" },
                new Supplier { SupplierID = 2, CompanyName = "Supplier B" }
            };

            _categoryRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(categories);
            _supplierRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(suppliers);
            _productRepositoryMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>()))
                .Returns(Task.CompletedTask);
            _productRepositoryMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.BulkGenerateProductsAsync(100);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.ProductsCreated);
            Assert.True(result.ElapsedMilliseconds >= 0);
            Assert.Contains("100 products", result.Message);
        }

        [Fact]
        public async Task BulkGenerateProductsAsync_WithLargeCount_UsesBatching()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { CategoryID = 1, CategoryName = "Electronics" }
            };

            var suppliers = new List<Supplier>
            {
                new Supplier { SupplierID = 1, CompanyName = "Supplier A" }
            };

            _categoryRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(categories);
            _supplierRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(suppliers);
            _productRepositoryMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>()))
                .Returns(Task.CompletedTask);
            _productRepositoryMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.BulkGenerateProductsAsync(25000);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(25000, result.ProductsCreated);
            // Verify batching: 25000 / 10000 = 3 batches
            _productRepositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>()), Times.Exactly(3));
            _productRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Exactly(3));
        }

        [Fact]
        public async Task BulkGenerateProductsAsync_WithoutCategories_ThrowsInvalidOperationException()
        {
            // Arrange
            _categoryRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Category>());
            _supplierRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Supplier> { new Supplier { SupplierID = 1 } });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _productService.BulkGenerateProductsAsync(100)
            );
            Assert.Contains("categories and suppliers", exception.Message);
        }

        [Fact]
        public async Task BulkGenerateProductsAsync_WithoutSuppliers_ThrowsInvalidOperationException()
        {
            // Arrange
            _categoryRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Category> { new Category { CategoryID = 1 } });
            _supplierRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Supplier>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _productService.BulkGenerateProductsAsync(100)
            );
            Assert.Contains("categories and suppliers", exception.Message);
        }
    }
}
