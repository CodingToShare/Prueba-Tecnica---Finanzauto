using System.Net;
using System.Net.Http.Json;
using ProductCatalog.Application.DTOs;
using Xunit;

namespace ProductCatalog.Tests.Integration
{
    public class CategoriesControllerTests
    {
        private CustomWebApplicationFactory<Program> CreateFactory()
        {
            return new CustomWebApplicationFactory<Program>();
        }

        [Fact]
        public async Task GetCategories_WithoutAuth_ReturnsOk()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/categories");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var categories = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
            Assert.NotNull(categories);
            Assert.True(categories.Count() >= 2); // We seeded 2 categories
        }

        [Fact]
        public async Task GetCategoryById_WithExistingId_ReturnsCategory()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/categories/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
            Assert.NotNull(category);
            Assert.Equal(1, category.CategoryID);
            Assert.Equal("Electronics", category.CategoryName);
        }

        [Fact]
        public async Task GetCategoryById_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/categories/9999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateCategory_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var createDto = new CreateCategoryDto
            {
                CategoryName = "New Category",
                Description = "Test description"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/categories", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateCategory_AsUser_ReturnsForbidden()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var token = await GetAuthToken(client, "user", "User123!");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var createDto = new CreateCategoryDto
            {
                CategoryName = "New Category",
                Description = "Test description"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/categories", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreateCategory_AsAdmin_ReturnsCreated()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var token = await GetAuthToken(client, "admin", "Admin123!");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var createDto = new CreateCategoryDto
            {
                CategoryName = "New Category",
                Description = "Test description"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/categories", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var category = await response.Content.ReadFromJsonAsync<CategoryDto>();
            Assert.NotNull(category);
            Assert.Equal("New Category", category.CategoryName);
        }

        [Fact]
        public async Task CreateCategory_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var token = await GetAuthToken(client, "admin", "Admin123!");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var createDto = new CreateCategoryDto
            {
                CategoryName = "", // Invalid: empty name
                Description = "Test"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/categories", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
