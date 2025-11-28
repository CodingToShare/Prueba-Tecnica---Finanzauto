# ProductCatalog API - Architecture Guide

Comprehensive guide to the architecture, design patterns, and principles used in the ProductCatalog API.

## Table of Contents

- [Overview](#overview)
- [Clean Architecture](#clean-architecture)
- [Layer Responsibilities](#layer-responsibilities)
- [Design Patterns](#design-patterns)
- [SOLID Principles](#solid-principles)
- [Project Structure](#project-structure)
- [Data Flow](#data-flow)
- [Security Architecture](#security-architecture)
- [Performance Considerations](#performance-considerations)
- [Architectural Decisions](#architectural-decisions)

---

## Overview

The ProductCatalog API is built using **Clean Architecture** principles, which provide:

✅ **Separation of Concerns** - Each layer has a specific responsibility
✅ **Testability** - Business logic can be tested independently
✅ **Maintainability** - Changes are localized to specific layers
✅ **Flexibility** - Easy to swap implementations
✅ **Independence** - Domain logic has no external dependencies

### Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                        External World                        │
│         (HTTP Requests, Database, External APIs)            │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│                     API Layer (Presentation)                 │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Controllers (AuthController, ProductsController)    │  │
│  │  - Handle HTTP requests/responses                    │  │
│  │  - Input validation                                  │  │
│  │  - Authorization attributes                          │  │
│  │  - DTOs mapping                                      │  │
│  └──────────────────────┬───────────────────────────────┘  │
│                         │                                   │
│  ┌──────────────────────▼───────────────────────────────┐  │
│  │  Middleware                                          │  │
│  │  - Exception handling                                │  │
│  │  - JWT authentication                                │  │
│  │  - CORS                                              │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│                  Application Layer (Services)                │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Services (ProductService, AuthService)              │  │
│  │  - Business logic orchestration                      │  │
│  │  - Use case implementation                           │  │
│  │  - Transaction management                            │  │
│  │  - DTOs and mapping                                  │  │
│  └──────────────────────┬───────────────────────────────┘  │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│                     Domain Layer (Core)                      │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Entities (Product, Category, Supplier, User)        │  │
│  │  - Business entities                                 │  │
│  │  - Domain logic                                      │  │
│  │  - Business rules                                    │  │
│  │  - No dependencies                                   │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Interfaces (IRepository<T>, IAuthService)           │  │
│  │  - Contracts for infrastructure                      │  │
│  │  - Abstractions                                      │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│              Infrastructure Layer (Data Access)              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  DbContext (ApplicationDbContext)                    │  │
│  │  - EF Core configuration                             │  │
│  │  - Entity mappings                                   │  │
│  │  - Database migrations                               │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Repositories (Repository<T>)                        │  │
│  │  - Data access implementation                        │  │
│  │  - Query logic                                       │  │
│  │  - CRUD operations                                   │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  External Services                                   │  │
│  │  - Third-party integrations                          │  │
│  │  - File storage                                      │  │
│  └──────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
```

### Dependency Rule

**Critical Principle**: Dependencies point **inward** only.

```
API Layer ──────→ Application Layer
                        │
                        ▼
Application Layer ──→ Domain Layer
                        ▲
                        │
Infrastructure Layer ───┘
```

- **Domain Layer**: No dependencies (pure business logic)
- **Application Layer**: Depends only on Domain
- **Infrastructure Layer**: Depends on Domain (implements interfaces)
- **API Layer**: Depends on Application and Domain

---

## Clean Architecture

### What is Clean Architecture?

Clean Architecture is a software design philosophy that separates code into layers based on business logic proximity. The main goal is to make the system:

1. **Independent of Frameworks** - Business rules don't depend on frameworks
2. **Testable** - Business logic can be tested without UI, database, or external dependencies
3. **Independent of UI** - UI can change without affecting business logic
4. **Independent of Database** - Business logic doesn't know about the database
5. **Independent of External Services** - Business logic doesn't know about external APIs

### Benefits in ProductCatalog API

✅ **Easy Testing** - Can test business logic without database
✅ **Flexible Database** - Can switch from PostgreSQL to SQL Server easily
✅ **Framework Independence** - Not locked into ASP.NET Core
✅ **Clear Separation** - Each layer has well-defined responsibilities
✅ **Maintainability** - Changes are isolated to specific layers

---

## Layer Responsibilities

### 1. Domain Layer (`ProductCatalog.Domain`)

**Purpose**: Core business entities and rules

**Responsibilities**:
- Define business entities (Product, Category, Supplier, User)
- Contain business logic and rules
- Define repository interfaces
- Define service interfaces
- No dependencies on other layers

**Key Files**:
```
ProductCatalog.Domain/
├── Entities/
│   ├── Product.cs          # Product entity with business logic
│   ├── Category.cs         # Category entity
│   ├── Supplier.cs         # Supplier entity
│   ├── User.cs             # User entity with roles
│   └── Inventory.cs        # Inventory tracking
├── Interfaces/
│   ├── IRepository.cs      # Generic repository interface
│   ├── IAuthService.cs     # Authentication contract
│   └── IProductService.cs  # Product service contract
└── Enums/
    └── UserRole.cs         # User role enumeration
```

**Example - Product Entity**:
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Business rule: Price must be positive
    private decimal _price;
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
                throw new ArgumentException("Price cannot be negative");
            _price = value;
        }
    }

    // Business logic: Check if product is low stock
    public bool IsLowStock(int threshold = 10)
    {
        return StockQuantity <= threshold;
    }

    // Business rule: Product is available if in stock and not discontinued
    public bool IsAvailable => StockQuantity > 0 && !Discontinued;
}
```

**Design Principles**:
- Entities contain business logic relevant to themselves
- Validation rules are enforced in setters
- No dependencies on infrastructure
- Pure C# classes with no framework attributes

### 2. Application Layer (`ProductCatalog.Application`)

**Purpose**: Business logic orchestration and use cases

**Responsibilities**:
- Implement use cases (CreateProduct, UpdateProduct, etc.)
- Orchestrate domain entities
- Manage transactions
- Define and map DTOs
- Validate business rules

**Key Files**:
```
ProductCatalog.Application/
├── Services/
│   ├── ProductService.cs   # Product business logic
│   ├── AuthService.cs      # Authentication logic
│   └── CategoryService.cs  # Category business logic
├── DTOs/
│   ├── ProductDto.cs       # Product data transfer object
│   ├── CreateProductDto.cs # Create product request
│   ├── PagedResultDto.cs   # Pagination wrapper
│   └── LoginDto.cs         # Login credentials
└── Validators/
    └── ProductValidator.cs # FluentValidation rules
```

**Example - ProductService**:
```csharp
public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly ILogger<ProductService> _logger;

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        // Business logic orchestration

        // 1. Validate category exists
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
        if (category == null)
            throw new NotFoundException("Category not found");

        // 2. Create domain entity
        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price, // Entity enforces price > 0
            CategoryId = dto.CategoryId
        };

        // 3. Save to repository
        await _productRepository.AddAsync(product);

        // 4. Log operation
        _logger.LogInformation("Created product {ProductId}", product.Id);

        // 5. Return DTO
        return MapToDto(product);
    }
}
```

**Design Principles**:
- Services orchestrate domain entities
- DTOs separate internal models from API contracts
- No database-specific code (uses repository abstractions)
- Transaction management happens here

### 3. Infrastructure Layer (`ProductCatalog.Infrastructure`)

**Purpose**: External concerns and data access

**Responsibilities**:
- Implement repository interfaces
- Configure Entity Framework Core
- Database migrations
- External service integrations
- File storage implementation

**Key Files**:
```
ProductCatalog.Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs  # EF Core DbContext
│   ├── DbInitializer.cs         # Database seeding
│   └── Configurations/
│       ├── ProductConfiguration.cs  # Product entity mapping
│       └── UserConfiguration.cs     # User entity mapping
├── Repositories/
│   └── Repository.cs            # Generic repository implementation
└── Migrations/
    └── 20250101000000_InitialCreate.cs
```

**Example - ApplicationDbContext**:
```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entities using Fluent API
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());

        // Seed initial data
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin
            }
        );
    }
}
```

**Example - Repository Implementation**:
```csharp
public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<PagedResult<T>> GetPagedAsync(
        int page, int pageSize,
        Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>(items, totalCount, page, pageSize);
    }
}
```

**Design Principles**:
- Implements interfaces defined in Domain
- Contains all database-specific code
- Uses EF Core for data access
- Handles data persistence concerns

### 4. API Layer (`ProductCatalog.Api`)

**Purpose**: HTTP API and presentation

**Responsibilities**:
- Handle HTTP requests/responses
- Route requests to appropriate services
- Authentication and authorization
- Input validation
- Exception handling
- API documentation (Swagger)

**Key Files**:
```
ProductCatalog.Api/
├── Controllers/
│   ├── AuthController.cs       # Authentication endpoints
│   ├── ProductsController.cs   # Product CRUD endpoints
│   ├── CategoriesController.cs # Category endpoints
│   └── HealthController.cs     # Health checks
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
├── Program.cs                  # Application startup
└── appsettings.json           # Configuration
```

**Example - ProductsController**:
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // JWT authentication required
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    [HttpGet]
    [AllowAnonymous] // Public endpoint
    public async Task<ActionResult<PagedResultDto<ProductDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null)
    {
        // Validation
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Invalid pagination parameters");

        // Call service
        var result = await _productService.GetProductsAsync(
            page, pageSize, categoryId);

        // Return response
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // Admin only
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductDto dto)
    {
        // Model validation happens automatically
        var product = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product);
    }
}
```

**Design Principles**:
- Controllers are thin (delegate to services)
- DTOs for request/response bodies
- Attributes for routing and authorization
- No business logic in controllers

---

## Design Patterns

### 1. Repository Pattern

**Purpose**: Abstraction over data access

**Implementation**:
```csharp
// Interface (Domain Layer)
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

// Implementation (Infrastructure Layer)
public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    // ... implementation
}
```

**Benefits**:
- Abstracts data access logic
- Easy to mock for testing
- Can swap database without changing business logic
- Centralized query logic

### 2. Dependency Injection

**Purpose**: Inversion of Control for loose coupling

**Implementation**:
```csharp
// Program.cs - Service Registration
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Usage in Controller
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService; // Injected by framework
    }
}
```

**Benefits**:
- Loose coupling between components
- Easy to swap implementations
- Testability (can inject mocks)
- Centralized configuration

### 3. DTO (Data Transfer Object) Pattern

**Purpose**: Separate internal models from API contracts

**Implementation**:
```csharp
// Domain Entity (internal)
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Category Category { get; set; } // Navigation property
    public string InternalNotes { get; set; } // Sensitive data
}

// DTO (external)
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string CategoryName { get; set; } // Flattened
    // InternalNotes not exposed
}
```

**Benefits**:
- API stability (internal changes don't affect API)
- Security (hide sensitive data)
- Flexibility (reshape data for API needs)
- Versioning support

### 4. Service Layer Pattern

**Purpose**: Encapsulate business logic

**Implementation**:
```csharp
public class ProductService : IProductService
{
    private readonly IRepository<Product> _repository;

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        // Business logic orchestration
        // Validation, entity creation, persistence
    }
}
```

**Benefits**:
- Centralized business logic
- Reusable across controllers
- Easy to test
- Transaction management

### 5. Unit of Work Pattern

**Purpose**: Manage transactions across repositories

**Implementation**:
```csharp
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Inventory> _inventoryRepository;

    public async Task CreateProductWithInventoryAsync(CreateProductDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Create product
            var product = new Product { /* ... */ };
            await _productRepository.AddAsync(product);

            // Create inventory record
            var inventory = new Inventory { ProductId = product.Id };
            await _inventoryRepository.AddAsync(inventory);

            // Commit both or neither
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

**Benefits**:
- Atomicity across multiple operations
- Data consistency
- Centralized transaction management

### 6. Factory Pattern

**Purpose**: Object creation logic

**Implementation**:
```csharp
public static class ProductFactory
{
    public static Product CreateFromDto(CreateProductDto dto)
    {
        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            SupplierId = dto.SupplierId,
            StockQuantity = dto.StockQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Discontinued = false
        };
    }
}
```

---

## SOLID Principles

### S - Single Responsibility Principle

**Principle**: Each class should have one reason to change

**Example**:
```csharp
// ✅ Good - Each class has single responsibility
public class ProductService
{
    // Responsibility: Product business logic
}

public class ProductValidator
{
    // Responsibility: Product validation
}

public class ProductRepository
{
    // Responsibility: Product data access
}

// ❌ Bad - Multiple responsibilities
public class ProductManager
{
    // Business logic + validation + data access
}
```

**Application in ProductCatalog API**:
- Controllers handle HTTP concerns only
- Services handle business logic only
- Repositories handle data access only
- DTOs handle data transfer only

### O - Open/Closed Principle

**Principle**: Open for extension, closed for modification

**Example**:
```csharp
// ✅ Good - Can extend without modifying
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(int id);
}

public class Repository<T> : IRepository<T> { }

public class CachedRepository<T> : IRepository<T>
{
    private readonly IRepository<T> _innerRepository;
    // Add caching without modifying original
}

// Can add new repository implementations without changing existing code
```

**Application in ProductCatalog API**:
- Generic repository can be extended with specific repositories
- Services use interfaces, allowing new implementations
- Middleware pipeline can be extended

### L - Liskov Substitution Principle

**Principle**: Derived classes must be substitutable for base classes

**Example**:
```csharp
// ✅ Good - All implementations satisfy contract
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(int id);
}

public class Repository<T> : IRepository<T>
{
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
}

public class InMemoryRepository<T> : IRepository<T>
{
    public async Task<T?> GetByIdAsync(int id)
    {
        return await Task.FromResult(_dictionary.GetValueOrDefault(id));
    }
}

// Both can be used interchangeably
```

**Application in ProductCatalog API**:
- All repository implementations satisfy IRepository<T> contract
- Services can use any IRepository<T> implementation
- Tests use InMemoryRepository, production uses EF Core Repository

### I - Interface Segregation Principle

**Principle**: Clients shouldn't depend on interfaces they don't use

**Example**:
```csharp
// ✅ Good - Segregated interfaces
public interface IReadRepository<T>
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
}

public interface IWriteRepository<T>
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

// Read-only service only depends on read interface
public class ProductQueryService
{
    private readonly IReadRepository<Product> _repository;
}

// ❌ Bad - Fat interface
public interface IRepository<T>
{
    // 20 methods, clients forced to depend on all
}
```

**Application in ProductCatalog API**:
- IRepository<T> contains only essential operations
- Services can define specific interfaces (IProductService, IAuthService)
- Each interface has focused responsibility

### D - Dependency Inversion Principle

**Principle**: Depend on abstractions, not concretions

**Example**:
```csharp
// ✅ Good - Depends on abstraction
public class ProductService
{
    private readonly IRepository<Product> _repository; // Interface

    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }
}

// ❌ Bad - Depends on concrete class
public class ProductService
{
    private readonly Repository<Product> _repository; // Concrete class

    public ProductService()
    {
        _repository = new Repository<Product>(); // Hard dependency
    }
}
```

**Application in ProductCatalog API**:
- All layers depend on Domain interfaces
- Infrastructure implements interfaces
- Services injected via DI container
- Easy to swap implementations

---

## Project Structure

### Solution Organization

```
ProductCatalog/
├── ProductCatalog.Domain/           # Core business logic
│   ├── Entities/
│   ├── Interfaces/
│   └── Enums/
│
├── ProductCatalog.Application/      # Use cases and services
│   ├── Services/
│   ├── DTOs/
│   └── Validators/
│
├── ProductCatalog.Infrastructure/   # Data access
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── DbInitializer.cs
│   │   └── Configurations/
│   ├── Repositories/
│   └── Migrations/
│
├── ProductCatalog.Api/              # HTTP API
│   ├── Controllers/
│   ├── Middleware/
│   ├── Program.cs
│   └── appsettings.json
│
└── ProductCatalog.Tests/            # Tests
    ├── Unit/
    │   ├── AuthServiceTests.cs
    │   └── ProductServiceTests.cs
    └── Integration/
        ├── AuthControllerTests.cs
        ├── ProductsControllerTests.cs
        └── CategoriesControllerTests.cs
```

### Assembly Dependencies

```
ProductCatalog.Api ──────────→ ProductCatalog.Application
                              ProductCatalog.Infrastructure
                              ProductCatalog.Domain

ProductCatalog.Application ──→ ProductCatalog.Domain

ProductCatalog.Infrastructure → ProductCatalog.Domain

ProductCatalog.Domain        # No dependencies
```

---

## Data Flow

### Request Flow - GET /api/products

```
1. HTTP Request
   │
   ▼
2. ProductsController.GetProducts()
   │ - Validate query parameters
   │ - Check authorization
   ▼
3. ProductService.GetProductsAsync()
   │ - Apply business rules
   │ - Filter criteria
   ▼
4. Repository<Product>.GetPagedAsync()
   │ - Build EF Core query
   │ - Apply pagination
   ▼
5. ApplicationDbContext
   │ - Execute SQL query
   │ - Map to entities
   ▼
6. Database (PostgreSQL)
   │ - Return data
   ▼
7. ProductService
   │ - Map entities to DTOs
   │ - Apply business logic
   ▼
8. ProductsController
   │ - Format response
   │ - Add headers
   ▼
9. HTTP Response (JSON)
```

### Request Flow - POST /api/products

```
1. HTTP Request (JSON body)
   │
   ▼
2. Model Binding & Validation
   │ - Bind JSON to CreateProductDto
   │ - Run Data Annotations validation
   ▼
3. ProductsController.CreateProduct()
   │ - Check authorization (Admin role)
   │ - Validate DTO
   ▼
4. ProductService.CreateProductAsync()
   │ - Validate business rules
   │ - Check category exists
   │ - Create domain entity
   ▼
5. Repository<Product>.AddAsync()
   │ - Add to DbSet
   │ - Mark as added
   ▼
6. ApplicationDbContext.SaveChangesAsync()
   │ - Begin transaction
   │ - Generate SQL INSERT
   │ - Commit transaction
   ▼
7. Database (PostgreSQL)
   │ - Insert record
   │ - Return generated ID
   ▼
8. ProductService
   │ - Map entity to DTO
   │ - Log operation
   ▼
9. ProductsController
   │ - Return 201 Created
   │ - Location header
   ▼
10. HTTP Response (JSON + headers)
```

### Authentication Flow

```
1. POST /api/auth/login
   │ - LoginDto (username, password)
   ▼
2. AuthService.LoginAsync()
   │ - Find user by username
   │ - Verify password with BCrypt
   ▼
3. JWT Token Generation
   │ - Create claims (username, role, id)
   │ - Sign with secret key
   │ - Set expiration (24 hours)
   ▼
4. Return TokenDto
   │ - Token string
   │ - Expiration
   ▼
5. Client stores token
   │
   ▼
6. Subsequent requests
   │ - Authorization: Bearer {token}
   ▼
7. JWT Middleware
   │ - Validate signature
   │ - Check expiration
   │ - Extract claims
   ▼
8. Set User.Identity
   │ - Claims principal
   │ - User roles
   ▼
9. [Authorize] Attribute
   │ - Check authentication
   │ - Check role requirements
   ▼
10. Controller Action Executes
```

---

## Security Architecture

### Authentication & Authorization

**JWT-Based Authentication**:
```csharp
// Token Generation
public class AuthService
{
    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        // 1. Validate credentials
        var user = await _userRepository.GetByUsernameAsync(dto.Username);
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials");

        // 2. Create claims
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        // 3. Generate token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new TokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo
        };
    }
}
```

**Authorization Levels**:
```csharp
// Public endpoint
[AllowAnonymous]
public async Task<ActionResult> GetProducts() { }

// Authenticated users only
[Authorize]
public async Task<ActionResult> CreateProduct() { }

// Admin only
[Authorize(Roles = "Admin")]
public async Task<ActionResult> DeleteProduct() { }
```

### Password Security

**BCrypt Hashing**:
```csharp
public class AuthService
{
    public async Task RegisterAsync(RegisterDto dto)
    {
        // Hash password with BCrypt (salt rounds: 11)
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = passwordHash, // Never store plain password
            Role = UserRole.User
        };

        await _userRepository.AddAsync(user);
    }
}
```

### Input Validation

**Multiple Validation Layers**:
```csharp
// 1. Data Annotations (DTO)
public class CreateProductDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, MinimumLength = 3)]
    public string Name { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
}

// 2. Entity Validation (Domain)
public class Product
{
    private decimal _price;
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
                throw new ArgumentException("Price cannot be negative");
            _price = value;
        }
    }
}

// 3. Business Validation (Service)
public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
{
    // Validate category exists
    var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
    if (category == null)
        throw new NotFoundException("Category not found");

    // Additional business rules
    if (dto.StockQuantity < 0)
        throw new ValidationException("Stock cannot be negative");
}
```

### OWASP Top 10 Mitigation

| Vulnerability | Mitigation in ProductCatalog API |
|--------------|----------------------------------|
| **A01: Broken Access Control** | JWT authentication, role-based authorization with [Authorize] attributes |
| **A02: Cryptographic Failures** | BCrypt password hashing, HTTPS enforcement, secure JWT signing |
| **A03: Injection** | EF Core parameterized queries, input validation |
| **A04: Insecure Design** | Clean Architecture, security by design principles |
| **A05: Security Misconfiguration** | Secure defaults, environment-based configuration |
| **A06: Vulnerable Components** | Dependabot, NuGet package updates |
| **A07: Authentication Failures** | JWT with expiration, BCrypt password policy |
| **A08: Data Integrity Failures** | Input validation, entity validation, DTOs |
| **A09: Logging Failures** | Structured logging, exception middleware |
| **A10: SSRF** | Input validation, URL validation (if applicable) |

---

## Performance Considerations

### Asynchronous Operations

**All I/O operations are async**:
```csharp
// ✅ Good - Async throughout the stack
public async Task<ProductDto> GetProductAsync(int id)
{
    var product = await _repository.GetByIdAsync(id); // Async DB call
    return MapToDto(product);
}

// ❌ Bad - Blocking calls
public ProductDto GetProduct(int id)
{
    var product = _repository.GetByIdAsync(id).Result; // Blocks thread
    return MapToDto(product);
}
```

**Benefits**:
- Better thread utilization
- Higher throughput
- Scalability under load

### Pagination

**Efficient data retrieval**:
```csharp
public async Task<PagedResult<Product>> GetPagedAsync(
    int page, int pageSize,
    Expression<Func<Product, bool>>? filter = null)
{
    IQueryable<Product> query = _dbSet;

    // Apply filter
    if (filter != null)
        query = query.Where(filter);

    // Count total (single query)
    var totalCount = await query.CountAsync();

    // Get page of data (single query)
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Product>(items, totalCount, page, pageSize);
}
```

**Benefits**:
- Reduces memory usage
- Faster response times
- Better user experience

### Batch Processing

**Bulk operations**:
```csharp
public async Task<int> BulkGenerateProductsAsync(int count)
{
    const int batchSize = 10000;
    var totalCreated = 0;

    for (int i = 0; i < count; i += batchSize)
    {
        var products = GenerateProducts(Math.Min(batchSize, count - i));

        // Add batch to context
        await _context.Products.AddRangeAsync(products);

        // Save batch
        await _context.SaveChangesAsync();

        totalCreated += products.Count;

        // Clear tracking for memory efficiency
        _context.ChangeTracker.Clear();
    }

    return totalCreated;
}
```

**Benefits**:
- Handles large datasets
- Prevents memory overflow
- Maintains performance

### Database Query Optimization

**Efficient queries**:
```csharp
// ✅ Good - Single query with Include
public async Task<Product?> GetProductWithCategoryAsync(int id)
{
    return await _context.Products
        .Include(p => p.Category)  // Eager loading
        .FirstOrDefaultAsync(p => p.Id == id);
}

// ❌ Bad - N+1 queries
public async Task<Product?> GetProductAsync(int id)
{
    var product = await _context.Products.FindAsync(id);
    product.Category = await _context.Categories.FindAsync(product.CategoryId);
    return product;
}
```

**Indexing Strategy**:
```csharp
modelBuilder.Entity<Product>()
    .HasIndex(p => p.CategoryId)      // Foreign key index
    .HasIndex(p => p.Name)            // Search index
    .HasIndex(p => p.Discontinued);   // Filter index
```

---

## Architectural Decisions

### 1. Why Clean Architecture?

**Decision**: Use Clean Architecture over traditional N-Tier

**Rationale**:
- Business logic independence from frameworks
- Better testability (can test without database)
- Flexibility to change infrastructure
- Clear separation of concerns

**Trade-offs**:
- More layers = more complexity
- Learning curve for developers
- More files and interfaces

**Outcome**: Improved maintainability and testability justify the complexity.

### 2. Why Generic Repository?

**Decision**: Use generic `Repository<T>` pattern

**Rationale**:
- Reduces code duplication
- Consistent data access pattern
- Easy to mock for testing
- Centralized query logic

**Trade-offs**:
- Less flexibility for complex queries
- May need specialized repositories for some entities

**Outcome**: Generic repository works well for CRUD operations. Can add specialized repositories when needed (e.g., `IProductRepository` for complex product queries).

### 3. Why PostgreSQL?

**Decision**: PostgreSQL over SQL Server or MySQL

**Rationale**:
- Open-source and free
- Excellent performance
- Strong data integrity
- JSON support for flexibility
- Docker-friendly

**Trade-offs**:
- Less Windows integration than SQL Server
- Smaller ecosystem than MySQL

**Outcome**: PostgreSQL provides the right balance of features, performance, and cost.

### 4. Why JWT for Authentication?

**Decision**: JWT tokens over session-based authentication

**Rationale**:
- Stateless (no server-side session storage)
- Scalable (works across multiple servers)
- Mobile-friendly
- Standard-based

**Trade-offs**:
- Cannot invalidate tokens before expiration
- Larger payloads than session IDs
- Token theft concerns

**Outcome**: JWT is suitable for RESTful API. Can add refresh tokens for better security.

### 5. Why DTOs?

**Decision**: Use DTOs instead of exposing entities directly

**Rationale**:
- API stability (internal changes don't break API)
- Security (hide sensitive data)
- Flexibility (reshape data)
- Validation separation

**Trade-offs**:
- More code (entities + DTOs + mapping)
- Mapping overhead

**Outcome**: DTOs provide necessary abstraction between domain and API. AutoMapper could reduce mapping boilerplate in the future.

### 6. Why Docker?

**Decision**: Containerize with Docker

**Rationale**:
- Consistent environments (dev = prod)
- Easy deployment
- Scalability (orchestration with Kubernetes)
- Isolation

**Trade-offs**:
- Learning curve
- Resource overhead
- Complexity for simple deployments

**Outcome**: Docker provides deployment flexibility and consistency across environments.

### 7. Why GitHub Actions?

**Decision**: GitHub Actions for CI/CD over Jenkins/Azure DevOps

**Rationale**:
- Integrated with GitHub
- Free for public repos
- YAML-based configuration
- Large marketplace of actions

**Trade-offs**:
- Limited to GitHub
- Cost for private repos with heavy usage

**Outcome**: GitHub Actions provides excellent CI/CD for projects hosted on GitHub.

---

## Future Architecture Considerations

### Caching Layer

**Potential Implementation**:
```csharp
public class CachedProductService : IProductService
{
    private readonly IProductService _innerService;
    private readonly IDistributedCache _cache;

    public async Task<ProductDto?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (cached != null)
            return JsonSerializer.Deserialize<ProductDto>(cached);

        var product = await _innerService.GetProductAsync(id);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return product;
    }
}
```

### Event-Driven Architecture

**Potential Implementation**:
```csharp
public class ProductService : IProductService
{
    private readonly IEventBus _eventBus;

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = await _repository.AddAsync(entity);

        // Publish event
        await _eventBus.PublishAsync(new ProductCreatedEvent
        {
            ProductId = product.Id,
            Name = product.Name,
            CreatedAt = DateTime.UtcNow
        });

        return MapToDto(product);
    }
}
```

### CQRS (Command Query Responsibility Segregation)

**Potential Implementation**:
```csharp
// Commands (write operations)
public class CreateProductCommand
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class CreateProductCommandHandler
{
    public async Task<int> Handle(CreateProductCommand command)
    {
        // Write to database
    }
}

// Queries (read operations)
public class GetProductQuery
{
    public int Id { get; set; }
}

public class GetProductQueryHandler
{
    public async Task<ProductDto> Handle(GetProductQuery query)
    {
        // Read from database (or cache)
    }
}
```

### Microservices

**Potential Split**:
```
ProductCatalog API (Monolith)
    ↓
    ↓ Split into microservices:
    ↓
├── Product Service (products, categories)
├── Inventory Service (stock tracking)
├── Auth Service (authentication)
└── Order Service (future)
```

---

## Summary

The ProductCatalog API follows **Clean Architecture** principles with:

✅ **Clear Layer Separation** - Domain, Application, Infrastructure, API
✅ **SOLID Principles** - Single responsibility, dependency inversion
✅ **Design Patterns** - Repository, DI, DTO, Service Layer
✅ **Security First** - JWT, BCrypt, OWASP compliance
✅ **Performance** - Async/await, pagination, batch processing
✅ **Testability** - Layer isolation, interface abstractions
✅ **Maintainability** - Clear structure, consistent patterns

This architecture provides a solid foundation that can evolve from a monolith to microservices as requirements grow.

---

## Additional Resources

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://www.digitalocean.com/community/conceptual-articles/s-o-l-i-d-the-first-five-principles-of-object-oriented-design)
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)
- [EF Core Performance](https://docs.microsoft.com/en-us/ef/core/performance/)

---

**[← Back to Documentation](../README.md#documentation)**
