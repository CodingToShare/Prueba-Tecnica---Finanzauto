using Microsoft.Extensions.Configuration;
using Moq;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Services;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;
using Xunit;

namespace ProductCatalog.Tests.Unit
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();

            // Setup configuration mock
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("YourSuperSecretKeyThatShouldBeStoredInEnvironmentVariablesOrAzureKeyVault_MinimumLength32Characters!");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("ProductCatalogApi");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("ProductCatalogClient");
            _configurationMock.Setup(c => c["Jwt:ExpirationHours"]).Returns("24");

            _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Test123!");
            var user = new User
            {
                UserID = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync("testuser"))
                .ReturnsAsync(user);

            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = "Test123!"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("test@example.com", result.Email);
            Assert.Equal("User", result.Role);
            Assert.NotEmpty(result.Token);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
        {
            // Arrange
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
            var user = new User
            {
                UserID = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                Role = "User",
                IsActive = true
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync("testuser"))
                .ReturnsAsync(user);

            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = "WrongPassword"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentUser_ReturnsNull()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetByUsernameAsync("nonexistent"))
                .ReturnsAsync((User?)null);

            var loginRequest = new LoginRequestDto
            {
                Username = "nonexistent",
                Password = "Test123!"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithInactiveUser_ReturnsNull()
        {
            // Arrange
            var user = new User
            {
                UserID = 1,
                Username = "inactiveuser",
                Email = "inactive@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                Role = "User",
                IsActive = false
            };

            _userRepositoryMock.Setup(r => r.GetByUsernameAsync("inactiveuser"))
                .ReturnsAsync(user);

            var loginRequest = new LoginRequestDto
            {
                Username = "inactiveuser",
                Password = "Test123!"
            };

            // Act
            var result = await _authService.LoginAsync(loginRequest);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsUserDto()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.UsernameExistsAsync("newuser"))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.EmailExistsAsync("newuser@example.com"))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var registerRequest = new RegisterRequestDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Test123!",
                Role = "User"
            };

            // Act
            var result = await _authService.RegisterAsync(registerRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newuser", result.Username);
            Assert.Equal("newuser@example.com", result.Email);
            Assert.Equal("User", result.Role);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingUsername_ThrowsInvalidOperationException()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.UsernameExistsAsync("existinguser"))
                .ReturnsAsync(true);

            var registerRequest = new RegisterRequestDto
            {
                Username = "existinguser",
                Email = "new@example.com",
                Password = "Test123!",
                Role = "User"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authService.RegisterAsync(registerRequest)
            );
            Assert.Equal("Username already exists", exception.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.UsernameExistsAsync("newuser"))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.EmailExistsAsync("existing@example.com"))
                .ReturnsAsync(true);

            var registerRequest = new RegisterRequestDto
            {
                Username = "newuser",
                Email = "existing@example.com",
                Password = "Test123!",
                Role = "User"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authService.RegisterAsync(registerRequest)
            );
            Assert.Equal("Email already exists", exception.Message);
        }

        [Fact]
        public async Task RegisterAsync_HashesPassword_Correctly()
        {
            // Arrange
            User? capturedUser = null;
            _userRepositoryMock.Setup(r => r.UsernameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var registerRequest = new RegisterRequestDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "PlainPassword123!",
                Role = "User"
            };

            // Act
            await _authService.RegisterAsync(registerRequest);

            // Assert
            Assert.NotNull(capturedUser);
            Assert.NotEqual("PlainPassword123!", capturedUser.PasswordHash);
            Assert.True(BCrypt.Net.BCrypt.Verify("PlainPassword123!", capturedUser.PasswordHash));
        }
    }
}
