# ProductCatalog API - Development Guide

Complete guide for developers working on the ProductCatalog API.

## Table of Contents

- [Getting Started](#getting-started)
- [Development Environment Setup](#development-environment-setup)
- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Testing](#testing)
- [Debugging](#debugging)
- [Common Tasks](#common-tasks)
- [Troubleshooting](#troubleshooting)
- [Best Practices](#best-practices)

---

## Getting Started

### Prerequisites

Ensure you have the following installed:

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 10.0+ | Runtime and development |
| PostgreSQL | 16+ | Database (or use Docker) |
| Git | Latest | Version control |
| Docker Desktop | Latest | Containerization (optional) |
| IDE | Latest | VS Code, Visual Studio, or Rider |

### Quick Start

```bash
# Clone repository
git clone <repository-url>
cd Backend

# Restore dependencies
dotnet restore

# Update database
dotnet ef database update --project ProductCatalog.Infrastructure

# Run application
dotnet run --project ProductCatalog.Api

# API is available at https://localhost:5001
```

---

## Development Environment Setup

### Option 1: Local Development

#### Install .NET 10 SDK

**Windows**:
```powershell
# Download from https://dotnet.microsoft.com/download/dotnet/10.0
winget install Microsoft.DotNet.SDK.10
```

**macOS**:
```bash
brew install dotnet@10
```

**Linux (Ubuntu)**:
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0
```

#### Install PostgreSQL 16

**Windows**:
```powershell
# Download from https://www.postgresql.org/download/windows/
# Or use chocolatey
choco install postgresql16
```

**macOS**:
```bash
brew install postgresql@16
brew services start postgresql@16
```

**Linux (Ubuntu)**:
```bash
sudo apt update
sudo apt install postgresql-16 postgresql-contrib-16
sudo systemctl start postgresql
sudo systemctl enable postgresql
```

#### Configure PostgreSQL

```bash
# Connect to PostgreSQL
psql -U postgres

# Create database
CREATE DATABASE ProductCatalogDb;

# Create user (optional)
CREATE USER productcatalog WITH PASSWORD 'yourpassword';
GRANT ALL PRIVILEGES ON DATABASE ProductCatalogDb TO productcatalog;

# Exit
\q
```

#### Update Connection String

Edit `ProductCatalog.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ProductCatalogDb;Username=postgres;Password=postgres"
  }
}
```

#### Run Migrations

```bash
# From solution root
dotnet ef database update --project ProductCatalog.Infrastructure
```

#### Run Application

```bash
dotnet run --project ProductCatalog.Api
```

**Access**:
- API: https://localhost:5001
- Swagger UI: https://localhost:5001/openapi/index.html

### Option 2: Docker Development

#### Prerequisites

- Docker Desktop installed and running

#### Quick Start

```bash
# Copy environment file
cp .env.example .env

# Start all services
docker compose up -d

# Check health
curl http://localhost:5000/health

# View logs
docker compose logs -f api
```

**Access**:
- API: http://localhost:5000
- Swagger UI: http://localhost:5000/openapi/index.html
- PgAdmin: http://localhost:5050 (admin@admin.com / admin)

#### Development with Hot Reload

```bash
# Use override file for hot reload
docker compose -f docker-compose.yml -f docker-compose.override.yml up

# Make code changes - they will be reflected immediately
```

### IDE Setup

#### Visual Studio Code

**Required Extensions**:
```bash
# Install C# extension
code --install-extension ms-dotnettools.csharp

# Install C# Dev Kit
code --install-extension ms-dotnettools.csdevkit

# Install Docker extension
code --install-extension ms-azuretools.vscode-docker

# Install PostgreSQL extension (optional)
code --install-extension ckolkman.vscode-postgres
```

**Recommended Settings** (`.vscode/settings.json`):
```json
{
  "dotnet.defaultSolution": "ProductCatalog.sln",
  "omnisharp.enableEditorConfigSupport": true,
  "omnisharp.enableRoslynAnalyzers": true,
  "files.exclude": {
    "**/bin": true,
    "**/obj": true
  }
}
```

**Launch Configuration** (`.vscode/launch.json`):
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Launch API",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/ProductCatalog.Api/bin/Debug/net10.0/ProductCatalog.Api.dll",
      "args": [],
      "cwd": "${workspaceFolder}/ProductCatalog.Api",
      "stopAtEntry": false,
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  ]
}
```

#### Visual Studio 2022

1. Open `ProductCatalog.sln`
2. Set `ProductCatalog.Api` as startup project
3. Press F5 to run with debugging
4. Access Swagger UI at https://localhost:5001/openapi/index.html

#### JetBrains Rider

1. Open `ProductCatalog.sln`
2. Configure run configuration for `ProductCatalog.Api`
3. Press F5 to run with debugging

---

## Project Structure

```
ProductCatalog/
├── ProductCatalog.Domain/           # Core business logic
│   ├── Entities/                   # Business entities
│   │   ├── Product.cs
│   │   ├── Category.cs
│   │   ├── Supplier.cs
│   │   ├── User.cs
│   │   └── Inventory.cs
│   ├── Interfaces/                 # Repository and service interfaces
│   │   ├── IRepository.cs
│   │   ├── IProductService.cs
│   │   └── IAuthService.cs
│   └── Enums/
│       └── UserRole.cs
│
├── ProductCatalog.Application/      # Business logic & use cases
│   ├── Services/                   # Service implementations
│   │   ├── ProductService.cs
│   │   ├── AuthService.cs
│   │   └── CategoryService.cs
│   └── DTOs/                       # Data transfer objects
│       ├── ProductDto.cs
│       ├── CreateProductDto.cs
│       ├── PagedResultDto.cs
│       └── LoginDto.cs
│
├── ProductCatalog.Infrastructure/   # Data access & external concerns
│   ├── Data/
│   │   ├── ApplicationDbContext.cs # EF Core context
│   │   ├── DbInitializer.cs       # Database seeding
│   │   └── Configurations/        # Entity configurations
│   ├── Repositories/
│   │   └── Repository.cs          # Generic repository
│   └── Migrations/                # EF Core migrations
│
├── ProductCatalog.Api/              # HTTP API layer
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ProductsController.cs
│   │   ├── CategoriesController.cs
│   │   └── HealthController.cs
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Program.cs                 # Application startup
│   ├── appsettings.json
│   └── appsettings.Development.json
│
├── ProductCatalog.Tests/            # Tests
│   ├── Unit/                       # Unit tests
│   │   ├── AuthServiceTests.cs
│   │   └── ProductServiceTests.cs
│   └── Integration/                # Integration tests
│       ├── AuthControllerTests.cs
│       ├── ProductsControllerTests.cs
│       └── CategoriesControllerTests.cs
│
├── docker-compose.yml               # Docker orchestration
├── Dockerfile                       # Docker image definition
├── Makefile                        # Development commands
└── ProductCatalog.sln              # Solution file
```

---

## Development Workflow

### Feature Development

#### 1. Create Feature Branch

```bash
# Update develop branch
git checkout develop
git pull origin develop

# Create feature branch
git checkout -b feature/product-search
```

#### 2. Implement Feature

**Add Entity** (Domain Layer):
```csharp
// ProductCatalog.Domain/Entities/Product.cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // ...
}
```

**Add Service** (Application Layer):
```csharp
// ProductCatalog.Application/Services/ProductService.cs
public class ProductService : IProductService
{
    public async Task<ProductDto> GetProductAsync(int id)
    {
        // Implementation
    }
}
```

**Add Controller** (API Layer):
```csharp
// ProductCatalog.Api/Controllers/ProductsController.cs
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        // Implementation
    }
}
```

#### 3. Add Tests

**Unit Test**:
```csharp
// ProductCatalog.Tests/Unit/ProductServiceTests.cs
public class ProductServiceTests
{
    [Fact]
    public async Task GetProductAsync_ReturnsProduct_WhenExists()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        // ...

        // Act
        var result = await service.GetProductAsync(1);

        // Assert
        result.Should().NotBeNull();
    }
}
```

**Integration Test**:
```csharp
// ProductCatalog.Tests/Integration/ProductsControllerTests.cs
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetProduct_ReturnsProduct_WhenExists()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/products/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

#### 4. Run Tests

```bash
# Run all tests
dotnet test

# Run specific test category
dotnet test --filter Category=Unit
dotnet test --filter Category=Integration

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

#### 5. Commit Changes

```bash
# Stage changes
git add .

# Commit with conventional commit message
git commit -m "feat: add product search functionality"

# Push to remote
git push origin feature/product-search
```

#### 6. Create Pull Request

```bash
# Using GitHub CLI
gh pr create --title "feat: add product search" --body "Adds search functionality for products by name and description"

# Or create PR on GitHub UI
```

### Database Changes

#### 1. Modify Entity

```csharp
// ProductCatalog.Domain/Entities/Product.cs
public class Product
{
    // Add new property
    public string? ImageUrl { get; set; }
}
```

#### 2. Create Migration

```bash
# Create migration
dotnet ef migrations add AddImageUrlToProduct --project ProductCatalog.Infrastructure

# Review migration file
# Migrations/YYYYMMDDHHMMSS_AddImageUrlToProduct.cs
```

#### 3. Apply Migration

```bash
# Update database
dotnet ef database update --project ProductCatalog.Infrastructure

# Or rollback
dotnet ef database update PreviousMigration --project ProductCatalog.Infrastructure
```

#### 4. Remove Migration (if not applied)

```bash
dotnet ef migrations remove --project ProductCatalog.Infrastructure
```

---

## Coding Standards

### Naming Conventions

**Pascal Case** - Classes, methods, properties, public fields:
```csharp
public class ProductService
{
    public async Task<Product> GetProductAsync(int id) { }
    public string ProductName { get; set; }
}
```

**Camel Case** - Private fields, parameters, local variables:
```csharp
private readonly IRepository<Product> _productRepository;
public void Method(int productId)
{
    var productName = "Example";
}
```

**Constants** - UPPER_SNAKE_CASE or PascalCase:
```csharp
public const int MAX_PAGE_SIZE = 100;
private const string DefaultCategory = "Uncategorized";
```

### File Organization

```csharp
// 1. Using statements (sorted)
using System;
using System.Collections.Generic;
using ProductCatalog.Domain.Entities;

// 2. Namespace
namespace ProductCatalog.Application.Services;

// 3. Class documentation (optional)
/// <summary>
/// Service for managing products
/// </summary>
public class ProductService : IProductService
{
    // 4. Private fields
    private readonly IRepository<Product> _repository;
    private readonly ILogger<ProductService> _logger;

    // 5. Constructor
    public ProductService(
        IRepository<Product> repository,
        ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // 6. Public methods
    public async Task<ProductDto> GetProductAsync(int id)
    {
        // Implementation
    }

    // 7. Private methods
    private ProductDto MapToDto(Product product)
    {
        // Implementation
    }
}
```

### Code Style

**Braces**:
```csharp
// ✅ Good - Always use braces
if (condition)
{
    DoSomething();
}

// ❌ Bad
if (condition)
    DoSomething();
```

**Async Naming**:
```csharp
// ✅ Good - Async suffix
public async Task<Product> GetProductAsync(int id)

// ❌ Bad
public async Task<Product> GetProduct(int id)
```

**Null Handling**:
```csharp
// ✅ Good - Null coalescing
var name = product?.Name ?? "Unknown";

// ✅ Good - Null check
if (product == null)
    throw new NotFoundException("Product not found");

// ❌ Bad
var name = product.Name; // Potential NullReferenceException
```

**LINQ Formatting**:
```csharp
// ✅ Good - Multi-line for readability
var products = await _context.Products
    .Where(p => !p.Discontinued)
    .Include(p => p.Category)
    .OrderBy(p => p.Name)
    .ToListAsync();

// ❌ Bad - Single long line
var products = await _context.Products.Where(p => !p.Discontinued).Include(p => p.Category).OrderBy(p => p.Name).ToListAsync();
```

### Comments

```csharp
// ✅ Good - Explain WHY, not WHAT
// Use BCrypt for password hashing to prevent rainbow table attacks
var hash = BCrypt.Net.BCrypt.HashPassword(password);

// ✅ Good - Document public API
/// <summary>
/// Retrieves a product by its unique identifier
/// </summary>
/// <param name="id">The product ID</param>
/// <returns>The product DTO or null if not found</returns>
public async Task<ProductDto?> GetProductAsync(int id)

// ❌ Bad - Obvious comment
// Get product by id
var product = await GetProductAsync(id);
```

---

## Testing

### Test Organization

```
ProductCatalog.Tests/
├── Unit/                           # Unit tests (no database)
│   ├── AuthServiceTests.cs        # Test AuthService methods
│   └── ProductServiceTests.cs     # Test ProductService methods
├── Integration/                    # Integration tests (with database)
│   ├── AuthControllerTests.cs     # Test Auth API endpoints
│   ├── ProductsControllerTests.cs # Test Products API endpoints
│   └── CategoriesControllerTests.cs
└── CustomWebApplicationFactory.cs # Test server factory
```

### Writing Unit Tests

**Arrange-Act-Assert Pattern**:
```csharp
[Fact]
public async Task GetProductAsync_ReturnsProduct_WhenExists()
{
    // Arrange
    var mockRepo = new Mock<IRepository<Product>>();
    mockRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new Product { Id = 1, Name = "Test Product" });

    var service = new ProductService(mockRepo.Object);

    // Act
    var result = await service.GetProductAsync(1);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(1);
    result.Name.Should().Be("Test Product");
}
```

**Test Naming**:
```
MethodName_ExpectedBehavior_Condition
```

Examples:
- `GetProductAsync_ReturnsProduct_WhenExists`
- `CreateProductAsync_ThrowsException_WhenCategoryNotFound`
- `Login_ReturnsToken_WhenCredentialsValid`

### Writing Integration Tests

```csharp
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ProductsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessStatusCode()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test file
dotnet test --filter ProductServiceTests

# Run specific test
dotnet test --filter GetProductAsync_ReturnsProduct_WhenExists

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
```

---

## Debugging

### Visual Studio Code

**Launch Configuration**:
```json
{
  "name": "Debug API",
  "type": "coreclr",
  "request": "launch",
  "preLaunchTask": "build",
  "program": "${workspaceFolder}/ProductCatalog.Api/bin/Debug/net10.0/ProductCatalog.Api.dll",
  "env": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  },
  "serverReadyAction": {
    "action": "openExternally",
    "pattern": "Now listening on: (https?://\\S+)"
  }
}
```

**Breakpoints**:
1. Click left margin to set breakpoint
2. Press F5 to start debugging
3. Make API request to trigger breakpoint

### Visual Studio 2022

1. Set breakpoint (F9)
2. Press F5 to start debugging
3. Step through code (F10 - step over, F11 - step into)

### Docker Debugging

**Attach to running container**:
```bash
# Get container ID
docker ps

# Attach debugger
docker exec -it <container-id> /bin/bash
```

### Logging

**Enable detailed logging**:
```json
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

**Add logging to code**:
```csharp
public class ProductService
{
    private readonly ILogger<ProductService> _logger;

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        _logger.LogInformation("Creating product: {ProductName}", dto.Name);

        try
        {
            var product = await _repository.AddAsync(entity);
            _logger.LogInformation("Created product with ID: {ProductId}", product.Id);
            return MapToDto(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product: {ProductName}", dto.Name);
            throw;
        }
    }
}
```

---

## Common Tasks

### Add New Entity

1. **Create Entity** (Domain):
```csharp
// ProductCatalog.Domain/Entities/Brand.cs
public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

2. **Add DbSet** (Infrastructure):
```csharp
// ApplicationDbContext.cs
public DbSet<Brand> Brands => Set<Brand>();
```

3. **Create Migration**:
```bash
dotnet ef migrations add AddBrandEntity --project ProductCatalog.Infrastructure
dotnet ef database update --project ProductCatalog.Infrastructure
```

### Add New API Endpoint

1. **Create DTO** (Application):
```csharp
public class BrandDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

2. **Add Service Method** (Application):
```csharp
public interface IBrandService
{
    Task<IEnumerable<BrandDto>> GetBrandsAsync();
}
```

3. **Add Controller** (API):
```csharp
[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandDto>>> GetBrands()
    {
        // Implementation
    }
}
```

4. **Add Tests**:
```csharp
[Fact]
public async Task GetBrands_ReturnsAllBrands()
{
    // Test implementation
}
```

### Update Dependencies

```bash
# List outdated packages
dotnet list package --outdated

# Update specific package
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.0

# Update all packages (use with caution)
dotnet outdated --upgrade
```

### Generate Code Coverage

```bash
# Install coverage tool
dotnet tool install --global dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html

# Open report
open coveragereport/index.html
```

---

## Troubleshooting

### Database Connection Issues

**Problem**: Cannot connect to PostgreSQL

**Solutions**:
```bash
# Check PostgreSQL is running
sudo systemctl status postgresql  # Linux
brew services list               # macOS

# Test connection
psql -U postgres -d ProductCatalogDb

# Check connection string
# appsettings.json - verify Host, Port, Username, Password
```

### Migration Errors

**Problem**: Migration fails to apply

**Solutions**:
```bash
# Drop and recreate database (development only)
dotnet ef database drop --project ProductCatalog.Infrastructure --force
dotnet ef database update --project ProductCatalog.Infrastructure

# View pending migrations
dotnet ef migrations list --project ProductCatalog.Infrastructure

# Rollback to specific migration
dotnet ef database update MigrationName --project ProductCatalog.Infrastructure
```

### Build Errors

**Problem**: Build fails

**Solutions**:
```bash
# Clean solution
dotnet clean

# Restore packages
dotnet restore

# Rebuild
dotnet build

# Clear NuGet cache (if packages corrupted)
dotnet nuget locals all --clear
```

### Test Failures

**Problem**: Tests fail unexpectedly

**Solutions**:
```bash
# Run tests in sequence (not parallel)
dotnet test --logger "console;verbosity=detailed"

# Check test database isolation
# Ensure each test uses unique database name

# Clear test data
# Use [Fact] instead of [Theory] to debug specific case
```

### Docker Issues

**Problem**: Docker build fails

**Solutions**:
```bash
# Clear Docker cache
docker system prune -a

# Rebuild without cache
docker compose build --no-cache

# View build logs
docker compose build --progress=plain
```

---

## Best Practices

### Do's

✅ **Write tests first** (TDD) when possible
✅ **Use async/await** for all I/O operations
✅ **Follow SOLID principles**
✅ **Use dependency injection**
✅ **Log important operations**
✅ **Handle exceptions properly**
✅ **Validate input at all layers**
✅ **Use DTOs for API contracts**
✅ **Document public APIs**
✅ **Keep controllers thin**

### Don'ts

❌ **Don't expose entities directly** in API
❌ **Don't use `.Result` or `.Wait()`** on async operations
❌ **Don't catch and swallow exceptions**
❌ **Don't use magic strings** (use constants)
❌ **Don't hardcode configuration**
❌ **Don't skip input validation**
❌ **Don't commit secrets** to Git
❌ **Don't use `SELECT *`** in queries
❌ **Don't ignore warnings**
❌ **Don't skip code review**

### Performance Tips

```csharp
// ✅ Use AsNoTracking for read-only queries
var products = await _context.Products
    .AsNoTracking()
    .ToListAsync();

// ✅ Use Include for eager loading
var product = await _context.Products
    .Include(p => p.Category)
    .FirstOrDefaultAsync(p => p.Id == id);

// ✅ Use pagination
var products = await _context.Products
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

// ✅ Use batch operations
await _context.Products.AddRangeAsync(products);
await _context.SaveChangesAsync();
```

### Security Tips

```csharp
// ✅ Always validate input
if (string.IsNullOrWhiteSpace(dto.Name))
    throw new ValidationException("Name is required");

// ✅ Use parameterized queries (EF Core does this automatically)
var product = await _context.Products
    .Where(p => p.Id == id)
    .FirstOrDefaultAsync();

// ✅ Hash passwords with BCrypt
var hash = BCrypt.Net.BCrypt.HashPassword(password);

// ✅ Use [Authorize] attribute
[Authorize(Roles = "Admin")]
public async Task<ActionResult> DeleteProduct(int id)
```

---

## Additional Resources

### Documentation
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [xUnit Testing](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)

### Tools
- [Postman](https://www.postman.com/) - API testing
- [pgAdmin](https://www.pgadmin.org/) - PostgreSQL management
- [DBeaver](https://dbeaver.io/) - Universal database tool
- [dotnet-outdated](https://github.com/dotnet-outdated/dotnet-outdated) - Check outdated packages

### Learning Resources
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://www.digitalocean.com/community/conceptual-articles/s-o-l-i-d-the-first-five-principles-of-object-oriented-design)
- [REST API Best Practices](https://docs.microsoft.com/en-us/azure/architecture/best-practices/api-design)

---

## Getting Help

1. **Check Documentation**: docs/ folder
2. **Search Issues**: GitHub Issues
3. **Ask Team**: Team chat or email
4. **Community**: Stack Overflow with tags: `.net`, `asp.net-core`, `entity-framework-core`

---

**[← Back to Documentation](../README.md#documentation)**
