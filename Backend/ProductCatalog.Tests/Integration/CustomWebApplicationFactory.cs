using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Tests.Integration
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private static readonly object _lock = new object();
        private static int _databaseCounter = 0;

        public CustomWebApplicationFactory()
        {
            // Set environment variable before the host is built
            Environment.SetEnvironmentVariable("ASPNETCORE_TESTING_SKIP_DBCONTEXT", "true");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureServices(services =>
            {
                // Use a unique database name for each test to avoid conflicts
                string dbName;
                lock (_lock)
                {
                    dbName = $"TestDatabase_{++_databaseCounter}";
                }

                // Add DbContext using in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                    options.EnableSensitiveDataLogging();
                });
            });

            // Seed data after the host is built
            builder.ConfigureServices(services =>
            {
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                    try
                    {
                        db.Database.EnsureCreated();
                        SeedTestData(db);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            });

            builder.UseEnvironment("Development");
        }

        private static void SeedTestData(ApplicationDbContext context)
        {
            // Clear existing data
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed test categories
            context.Categories.AddRange(
                new Domain.Entities.Category
                {
                    CategoryID = 1,
                    CategoryName = "Electronics",
                    Description = "Electronic devices"
                },
                new Domain.Entities.Category
                {
                    CategoryID = 2,
                    CategoryName = "Books",
                    Description = "Books and literature"
                }
            );

            // Seed test suppliers
            context.Suppliers.AddRange(
                new Domain.Entities.Supplier
                {
                    SupplierID = 1,
                    CompanyName = "Test Supplier A",
                    ContactName = "John Doe",
                    City = "New York",
                    Country = "USA"
                },
                new Domain.Entities.Supplier
                {
                    SupplierID = 2,
                    CompanyName = "Test Supplier B",
                    ContactName = "Jane Smith",
                    City = "London",
                    Country = "UK"
                }
            );

            // Seed test products
            context.Products.AddRange(
                new Domain.Entities.Product
                {
                    ProductID = 1,
                    ProductName = "Test Product 1",
                    SupplierID = 1,
                    CategoryID = 1,
                    UnitPrice = 99.99m,
                    UnitsInStock = 100,
                    Discontinued = false
                },
                new Domain.Entities.Product
                {
                    ProductID = 2,
                    ProductName = "Test Product 2",
                    SupplierID = 2,
                    CategoryID = 2,
                    UnitPrice = 49.99m,
                    UnitsInStock = 50,
                    Discontinued = false
                }
            );

            // Seed test users
            context.Users.AddRange(
                new Domain.Entities.User
                {
                    UserID = 1,
                    Username = "admin",
                    Email = "admin@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Domain.Entities.User
                {
                    UserID = 2,
                    Username = "user",
                    Email = "user@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                    Role = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            context.SaveChanges();
        }
    }
}
