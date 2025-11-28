using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ProductCatalog.Application.DTOs;
using Xunit;

namespace ProductCatalog.Tests.Integration
{
    public class AuthControllerTests
    {
        private CustomWebApplicationFactory<Program> CreateFactory()
        {
            return new CustomWebApplicationFactory<Program>();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var loginRequest = new LoginRequestDto
            {
                Username = "admin",
                Password = "Admin123!"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            Assert.NotNull(loginResponse);
            Assert.Equal("admin", loginResponse.Username);
            Assert.Equal("Admin", loginResponse.Role);
            Assert.NotEmpty(loginResponse.Token);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var loginRequest = new LoginRequestDto
            {
                Username = "admin",
                Password = "WrongPassword!"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var loginRequest = new LoginRequestDto
            {
                Username = "nonexistent",
                Password = "Test123!"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithInvalidInput_ReturnsBadRequest()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var loginRequest = new LoginRequestDto
            {
                Username = "ab", // Too short
                Password = "123"  // Too short
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_AsAdmin_WithValidData_ReturnsCreated()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();

            // First login as admin to get token
            var loginRequest = new LoginRequestDto
            {
                Username = "admin",
                Password = "Admin123!"
            };

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            Assert.NotNull(loginResult);

            // Set authorization header
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

            var registerRequest = new RegisterRequestDto
            {
                Username = "newuser",
                Email = "newuser@test.com",
                Password = "Test123!",
                Role = "User"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/auth/register", registerRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
            Assert.NotNull(userDto);
            Assert.Equal("newuser", userDto.Username);
            Assert.Equal("User", userDto.Role);
        }

        [Fact]
        public async Task Register_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();
            var registerRequest = new RegisterRequestDto
            {
                Username = "unauthorized",
                Email = "unauthorized@test.com",
                Password = "Test123!",
                Role = "User"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/auth/register", registerRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Register_AsUser_ReturnsForbidden()
        {
            // Arrange
            using var factory = CreateFactory();
            var client = factory.CreateClient();

            // Login as regular user
            var loginRequest = new LoginRequestDto
            {
                Username = "user",
                Password = "User123!"
            };

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            Assert.NotNull(loginResult);

            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

            var registerRequest = new RegisterRequestDto
            {
                Username = "anotheruser",
                Email = "another@test.com",
                Password = "Test123!",
                Role = "User"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/auth/register", registerRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
