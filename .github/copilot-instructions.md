# AI Coding Agent Instructions - Product Catalog API

## Project Architecture Overview

This is a **Clean Architecture** e-commerce product catalog backend (.NET 10.0) with clear layer separation and strict dependency rules.

### Layer Dependencies (strict unidirectional flow)
```
API → Application → Domain ← Infrastructure
    ↘_____________↙
```

- **Domain**: Core entities (Product, Category, Supplier, etc.) and abstract repository interfaces. NO external dependencies.
- **Application**: Service interfaces & implementations, DTOs (create/read/update), business logic orchestration. Depends on Domain only.
- **Infrastructure**: EF Core DbContext, concrete repositories, entity configurations, Npgsql PostgreSQL driver. Depends on Domain only.
- **Api**: ASP.NET Core controllers, HTTP routing, dependency injection wiring. Depends on Application & Infrastructure.

### Key Architectural Pattern: Generic + Specialized Repositories

All entities use the generic `Repository<T>` base class implementing `IRepository<T>`:
- CRUD operations: `GetByIdAsync()`, `GetAllAsync()`, `FindAsync()`, `AddAsync()`, `Update()`, `SaveChangesAsync()`

Specialized repositories override for business queries:
- `ProductRepository`: Adds `GetProductsWithDetailsAsync()` (with pagination, filtering, eager-loaded Category/Supplier), `CountAsync()` (for filtering), `GetProductByIdWithDetailsAsync()`
- Similar pattern for `CategoryRepository`, `SupplierRepository`, etc. (extend base Repository<T>)

## Critical Developer Workflows

### Build & Run
```powershell
# Build entire solution
dotnet build Backend/ProductCatalog.sln

# Run API (watches for changes in development)
dotnet run --project Backend/ProductCatalog.Api
dotnet watch --project Backend/ProductCatalog.Api
```

### Database Migrations (run from Backend directory)
```powershell
# Create migration after entity changes
dotnet ef migrations add MigrationName --project ProductCatalog.Infrastructure --startup-project ProductCatalog.Api

# Apply pending migrations to database
dotnet ef database update --project ProductCatalog.Infrastructure --startup-project ProductCatalog.Api

# Remove last migration if needed
dotnet ef migrations remove --project ProductCatalog.Infrastructure --startup-project ProductCatalog.Api
```

### Database Configuration
- **PostgreSQL** on localhost:5432, database `ProductCatalogDb`
- Credentials in `appsettings.json`: `postgres:postgres`
- Connection string: `Host=localhost;Port=5432;Database=ProductCatalogDb;Username=postgres;Password=postgres`

## Project-Specific Conventions

### Service Layer (Application)
1. **Services always inject repositories**: `ProductService` takes `IProductRepository`, `ICategoryRepository`, `ISupplierRepository` in constructor
2. **SaveChangesAsync() pattern**: Repositories must call `SaveChangesAsync()` after Add/Update/Remove operations
3. **DTO mapping responsibility**: Services map domain entities to DTOs (never let API directly return domain entities)
4. **Soft delete pattern**: `DeleteProductAsync()` sets `Discontinued = true` rather than hard delete

### DTO Strategy (Three-tier)
- **Read DTOs**: `ProductDto` (list view with related names), `ProductDetailDto` (full object with nested Category/Supplier objects)
- **Create/Update DTOs**: `CreateProductDto`, `UpdateProductDto` (subset of properties)
- **Response DTOs**: `PagedResultDto<T>` (pagination wrapper), `BulkInsertResultDto` (operation results)

Example from `ProductService.MapToDto()`:
```csharp
// Flat DTO for lists - includes denormalized names
public static ProductDto MapToDto(Product p) => new()
{
    ProductID = p.ProductID,
    CategoryName = p.Category?.CategoryName,  // String, not object
    SupplierName = p.Supplier?.CompanyName
};

// Nested DTO for detail view - includes full related objects
public static ProductDetailDto MapToDetailDto(Product p) => new()
{
    ProductID = p.ProductID,
    Category = p.Category != null ? new CategoryDto { ... } : null  // Full object
};
```

### Query Patterns (ProductRepository)
1. **Eager loading**: Use `.Include(p => p.Category).Include(p => p.Supplier)` to prevent N+1 queries
2. **Filtering composability**: Build queryable step-by-step (categoryId, price range, name search)
3. **Pagination**: Always `.Skip((page - 1) * pageSize).Take(pageSize)`
4. **Count method**: Separate `CountAsync()` with same filters for total record calculation

Example from `GetProductsWithDetailsAsync()`:
```csharp
var query = _context.Products
    .Include(p => p.Category)
    .Include(p => p.Supplier)
    .AsQueryable();

if (categoryId.HasValue) query = query.Where(p => p.CategoryID == categoryId);
if (minPrice.HasValue) query = query.Where(p => p.UnitPrice >= minPrice);
// Build dynamically, then paginate
return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
```

### Entity Configuration (EF Core Fluent API)
Configurations live in `Infrastructure/Data/Configurations/EntityConfigurations.cs` using `IEntityTypeConfiguration<T>`:
- Primary keys: `builder.HasKey()`
- Constraints: `.IsRequired()`, `.HasMaxLength(40)`
- Relationships: `.HasOne().WithMany().HasForeignKey()` (one-to-many)
- Composite keys: `builder.HasKey(od => new { od.OrderID, od.ProductID })` (OrderDetail)
- Indexes: `.HasIndex(u => u.Username).IsUnique()`

### Dependency Injection Pattern
- Application services registered in `ProductCatalog.Application.DependencyInjection.AddApplication()` as **Scoped**
- Infrastructure repositories registered in `ProductCatalog.Infrastructure.DependencyInjection.AddInfrastructure()` as **Scoped**
- Extension methods called in `Program.cs`: `builder.Services.AddApplication(); builder.Services.AddInfrastructure(builder.Configuration);`

### API Endpoint Patterns (ProductController)
- `POST /product` + `CreateProductDto` → returns `CreatedAtAction(nameof(GetById), ...)`
- `GET /product?page=1&pageSize=10&categoryId=5&minPrice=10&maxPrice=100&search=text` → returns `PagedResultDto<ProductDto>`
- `GET /product/{id}` → returns `ProductDetailDto` with nested objects, or 404
- `PUT /product/{id}` + `UpdateProductDto` → returns 204 NoContent, or 404
- `DELETE /product/{id}` → soft delete (sets Discontinued), returns 204

## Integration Points & External Dependencies

### PostgreSQL + Entity Framework Core 10.0
- Connection via `Npgsql` NuGet package in `ProductCatalog.Infrastructure.csproj`
- DbContext: `ApplicationDbContext` inherits from EF Core DbContext
- Migrations tracked in `Infrastructure/Migrations/`
- Decimal columns configured as `decimal(18,2)` for prices

### JWT Authentication (configured, not yet implemented in controllers)
- Settings in `appsettings.json`: Key, Issuer, Audience, ExpirationHours
- NuGet packages: `Microsoft.AspNetCore.Authentication.JwtBearer`, `System.IdentityModel.Tokens.Jwt`
- Token validation ready to be wired in `Program.cs` and applied to controllers via `[Authorize]`

### CORS Configuration
- Allowed origins in `appsettings.json`: `http://localhost:3000`, `http://localhost:5173`, `https://localhost:5001`
- Ready to enable with `.AddCors().UseCors()`

## Testing & Bulk Operations

### Bulk Insert Workflow
`ProductService.BulkGenerateProductsAsync()` generates random products in 10,000-item batches for performance testing:
1. Fetches all existing categories/suppliers
2. Validates they exist (throws if empty)
3. Generates products with random names, prices ($10-$1000), stock levels
4. Inserts batches via `AddRangeAsync()` + `SaveChangesAsync()`
5. Returns `BulkInsertResultDto` with count + elapsed time

This pattern is useful for load testing or demo data generation.

## Azure Deployment

### Infrastructure Setup
Run from project root to provision Azure resources:
- **PowerShell (Windows):** `.\azure-setup.ps1` 
- **Bash (Linux/macOS/WSL):** `./azure-setup.sh`

Creates:
- PostgreSQL Flexible Server v14 (Burstable B1ms)
- Azure Container Registry (ACR)
- App Service + Plan (Standard S1)
- Key Vault for secrets
- GitHub CI/CD pipeline ready

### Docker Build & Push
```powershell
docker build -t $ACR_URL/productcatalog-api:latest .
az acr login --name $ACR_NAME
docker push $ACR_URL/productcatalog-api:latest
```

### Database Migrations on Azure
```bash
dotnet ef database update \
  --project ProductCatalog.Infrastructure \
  --startup-project ProductCatalog.Api \
  --connection "Host=<postgres-fqdn>;Port=5432;Database=ProductCatalogDb;Username=adminuser@<server>;Password=<pwd>;SSL Mode=Require;"
```

### CI/CD Pipeline
- Workflow: `.github/workflows/azure-deploy-backend.yml`
- Triggers on: push to `main`/`develop`, changes in `Backend/**`
- Auto-builds Docker image → pushes to ACR → deploys to App Service → health checks

### Required GitHub Secrets
- `AZURE_CREDENTIALS` (Service Principal JSON)
- `AZURE_SUBSCRIPTION_ID`, `AZURE_RESOURCE_GROUP`, `AZURE_BACKEND_APP_NAME`
- `ACR_URL`, `ACR_USERNAME`, `ACR_PASSWORD`

**Reference docs:** `AZURE_DEPLOYMENT.md`, `AZURE_QUICK_START.md`, `DEPLOYMENT_CHECKLIST.md`

---

**When adding features:**
- New entities go in `Domain/Entities/`
- New services go in `Application/Services/`, with interface in `Application/Interfaces/`
- New repositories extend `Repository<T>` in `Infrastructure/Repositories/`
- New entity configs go in `Infrastructure/Data/Configurations/EntityConfigurations.cs`
- New DTOs go in `Application/DTOs/`
- Wire dependencies in `Program.cs` or extend `DependencyInjection.cs` methods
