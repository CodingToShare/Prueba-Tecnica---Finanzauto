using System.Net;
using System.Net.Http.Json;
using ProductCatalog.Application.DTOs;
using Xunit;

namespace ProductCatalog.Tests.Integration
{
    public class ProductsControllerTests
    {
        private CustomWebApplicationFactory<Program> CreateFactory()
        {
            return new CustomWebApplicationFactory<Program>();
        }

        [Fact]
        public async Task GetProducts_WithoutAuth_ReturnsOk()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/products?page=1&pageSize=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<PagedResultDto<ProductDto>>();
            Assert.NotNull(result);
            Assert.True(result.TotalCount >= 2); // We seeded 2 products
            Assert.NotEmpty(result.Items);
        }

        [Fact]
        public async Task GetProducts_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/products?page=1&pageSize=1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<PagedResultDto<ProductDto>>();
            Assert.NotNull(result);
            Assert.Equal(1, result.PageSize);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetProducts_WithCategoryFilter_ReturnsFilteredResults()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/products?categoryId=1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<PagedResultDto<ProductDto>>();
            Assert.NotNull(result);
            Assert.All(result.Items, item => Assert.Equal(1, item.CategoryID));
        }

        [Fact]
        public async Task GetProductById_WithExistingId_ReturnsProduct()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/products/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var product = await response.Content.ReadFromJsonAsync<ProductDetailDto>();
            Assert.NotNull(product);
            Assert.Equal(1, product.ProductID);
            Assert.Equal("Test Product 1", product.ProductName);
            Assert.NotNull(product.Category);
            Assert.NotNull(product.Supplier);
        }

        [Fact]
        public async Task GetProductById_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/products/9999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var updateDto = new UpdateProductDto
            {
                ProductName = "Updated Product",
                UnitPrice = 199.99m
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/products/1", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_AsUser_ReturnsOk()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var token = await GetAuthToken(client, "user", "User123!");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var updateDto = new UpdateProductDto
            {
                ProductName = "Updated by User",
                SupplierID = 1,
                CategoryID = 1,
                UnitPrice = 199.99m,
                UnitsInStock = 50
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/products/1", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<ProductDto>();
            Assert.NotNull(result);
            Assert.Equal("Updated by User", result.ProductName);
            Assert.Equal(199.99m, result.UnitPrice);
        }

        [Fact]
        public async Task DeleteProduct_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/products/1");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_AsUser_ReturnsForbidden()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var token = await GetAuthToken(client, "user", "User123!");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.DeleteAsync("/api/products/1");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_AsAdmin_ReturnsNoContent()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var token = await GetAuthToken(client, "admin", "Admin123!");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.DeleteAsync("/api/products/2");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify product is marked as discontinued (soft delete)
            var getResponse = await client.GetAsync("/api/products/2");
            var product = await getResponse.Content.ReadFromJsonAsync<ProductDetailDto>();
            Assert.NotNull(product);
            Assert.True(product.Discontinued);
        }

        [Fact]
        public async Task BulkGenerateProducts_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var bulkRequest = new BulkGenerateProductsDto { Count = 10 };

            // Act
            var response = await client.PostAsJsonAsync("/api/products/bulk", bulkRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task BulkGenerateProducts_AsUser_ReturnsOk()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var token = await GetAuthToken(client, "user", "User123!");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var bulkRequest = new BulkGenerateProductsDto { Count = 100 };

            // Act
            var response = await client.PostAsJsonAsync("/api/products/bulk", bulkRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<BulkInsertResultDto>();
            Assert.NotNull(result);
            Assert.Equal(100, result.ProductsCreated);
            Assert.True(result.ElapsedMilliseconds >= 0);
        }

        [Fact]
        public async Task BulkGenerateProducts_WithInvalidCount_ReturnsBadRequest()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var token = await GetAuthToken(client, "admin", "Admin123!");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var bulkRequest = new BulkGenerateProductsDto { Count = 150000 }; // Exceeds max

            // Act
            var response = await client.PostAsJsonAsync("/api/products/bulk", bulkRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task BulkGenerateProducts_WithLargeCount_CompletesSuccessfully()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var token = await GetAuthToken(client, "admin", "Admin123!");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var bulkRequest = new BulkGenerateProductsDto { Count = 5000 };

            // Act
            var response = await client.PostAsJsonAsync("/api/products/bulk", bulkRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<BulkInsertResultDto>();
            Assert.NotNull(result);
            Assert.Equal(5000, result.ProductsCreated);
            Assert.Contains("5000 products", result.Message);
        }

        private async Task<string> GetAuthToken(HttpClient client, string username, string password)
        {
            var loginRequest = new LoginRequestDto
            {
                Username = username,
                Password = password
            };

            var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
            var loginResult = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            return loginResult?.Token ?? throw new InvalidOperationException("Failed to get auth token");
        }
    }
}
