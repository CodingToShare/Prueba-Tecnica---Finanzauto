using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace ProductCatalog.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users (Admin and regular user)
            var users = new[]
            {
                new User
                {
                    UserID = 1,
                    Username = "admin",
                    PasswordHash = HashPassword("Admin123!"),
                    Email = "admin@productcatalog.com",
                    FirstName = "Admin",
                    LastName = "System",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserID = 2,
                    Username = "user",
                    PasswordHash = HashPassword("User123!"),
                    Email = "user@productcatalog.com",
                    FirstName = "Regular",
                    LastName = "User",
                    Role = "User",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            };
            modelBuilder.Entity<User>().HasData(users);

            // Seed Categories
            var categories = new[]
            {
                new Category { CategoryID = 1, CategoryName = "Beverages", Description = "Soft drinks, coffees, teas, beers, and ales" },
                new Category { CategoryID = 2, CategoryName = "Condiments", Description = "Sweet and savory sauces, relishes, spreads, and seasonings" },
                new Category { CategoryID = 3, CategoryName = "Confections", Description = "Desserts, candies, and sweet breads" },
                new Category { CategoryID = 4, CategoryName = "Dairy Products", Description = "Cheeses" },
                new Category { CategoryID = 5, CategoryName = "Grains/Cereals", Description = "Breads, crackers, pasta, and cereal" },
                new Category { CategoryID = 6, CategoryName = "Meat/Poultry", Description = "Prepared meats" },
                new Category { CategoryID = 7, CategoryName = "Produce", Description = "Dried fruit and bean curd" },
                new Category { CategoryID = 8, CategoryName = "Seafood", Description = "Seaweed and fish" }
            };
            modelBuilder.Entity<Category>().HasData(categories);

            // Seed Suppliers
            var suppliers = new[]
            {
                new Supplier
                {
                    SupplierID = 1,
                    CompanyName = "Exotic Liquids",
                    ContactName = "Charlotte Cooper",
                    ContactTitle = "Purchasing Manager",
                    Address = "49 Gilbert St.",
                    City = "London",
                    Region = null,
                    PostalCode = "EC1 4SD",
                    Country = "UK",
                    Phone = "(171) 555-2222"
                },
                new Supplier
                {
                    SupplierID = 2,
                    CompanyName = "New Orleans Cajun Delights",
                    ContactName = "Shelley Burke",
                    ContactTitle = "Order Administrator",
                    Address = "P.O. Box 78934",
                    City = "New Orleans",
                    Region = "LA",
                    PostalCode = "70117",
                    Country = "USA",
                    Phone = "(100) 555-4822"
                },
                new Supplier
                {
                    SupplierID = 3,
                    CompanyName = "Grandma Kelly's Homestead",
                    ContactName = "Regina Murphy",
                    ContactTitle = "Sales Representative",
                    Address = "707 Oxford Rd.",
                    City = "Ann Arbor",
                    Region = "MI",
                    PostalCode = "48104",
                    Country = "USA",
                    Phone = "(313) 555-5735"
                },
                new Supplier
                {
                    SupplierID = 4,
                    CompanyName = "Tokyo Traders",
                    ContactName = "Yoshi Nagase",
                    ContactTitle = "Marketing Manager",
                    Address = "9-8 Sekimai Musashino-shi",
                    City = "Tokyo",
                    Region = null,
                    PostalCode = "100",
                    Country = "Japan",
                    Phone = "(03) 3555-5011"
                },
                new Supplier
                {
                    SupplierID = 5,
                    CompanyName = "Cooperativa de Quesos 'Las Cabras'",
                    ContactName = "Antonio del Valle Saavedra",
                    ContactTitle = "Export Administrator",
                    Address = "Calle del Rosal 4",
                    City = "Oviedo",
                    Region = "Asturias",
                    PostalCode = "33007",
                    Country = "Spain",
                    Phone = "(98) 598 76 54"
                }
            };
            modelBuilder.Entity<Supplier>().HasData(suppliers);

            // Seed Products
            var products = new[]
            {
                new Product
                {
                    ProductID = 1,
                    ProductName = "Chai",
                    SupplierID = 1,
                    CategoryID = 1,
                    QuantityPerUnit = "10 boxes x 20 bags",
                    UnitPrice = 18.00m,
                    UnitsInStock = 39,
                    UnitsOnOrder = 0,
                    ReorderLevel = 10,
                    Discontinued = false
                },
                new Product
                {
                    ProductID = 2,
                    ProductName = "Chang",
                    SupplierID = 1,
                    CategoryID = 1,
                    QuantityPerUnit = "24 - 12 oz bottles",
                    UnitPrice = 19.00m,
                    UnitsInStock = 17,
                    UnitsOnOrder = 40,
                    ReorderLevel = 25,
                    Discontinued = false
                },
                new Product
                {
                    ProductID = 3,
                    ProductName = "Aniseed Syrup",
                    SupplierID = 1,
                    CategoryID = 2,
                    QuantityPerUnit = "12 - 550 ml bottles",
                    UnitPrice = 10.00m,
                    UnitsInStock = 13,
                    UnitsOnOrder = 70,
                    ReorderLevel = 25,
                    Discontinued = false
                },
                new Product
                {
                    ProductID = 4,
                    ProductName = "Chef Anton's Cajun Seasoning",
                    SupplierID = 2,
                    CategoryID = 2,
                    QuantityPerUnit = "48 - 6 oz jars",
                    UnitPrice = 22.00m,
                    UnitsInStock = 53,
                    UnitsOnOrder = 0,
                    ReorderLevel = 0,
                    Discontinued = false
                },
                new Product
                {
                    ProductID = 5,
                    ProductName = "Chef Anton's Gumbo Mix",
                    SupplierID = 2,
                    CategoryID = 2,
                    QuantityPerUnit = "36 boxes",
                    UnitPrice = 21.35m,
                    UnitsInStock = 0,
                    UnitsOnOrder = 0,
                    ReorderLevel = 0,
                    Discontinued = true
                },
                new Product
                {
                    ProductID = 6,
                    ProductName = "Grandma's Boysenberry Spread",
                    SupplierID = 3,
                    CategoryID = 2,
                    QuantityPerUnit = "12 - 8 oz jars",
                    UnitPrice = 25.00m,
                    UnitsInStock = 120,
                    UnitsOnOrder = 0,
                    ReorderLevel = 25,
                    Discontinued = false
                },
                new Product
                {
                    ProductID = 7,
                    ProductName = "Uncle Bob's Organic Dried Pears",
                    SupplierID = 3,
                    CategoryID = 7,
                    QuantityPerUnit = "12 - 1 lb pkgs.",
                    UnitPrice = 30.00m,
                    UnitsInStock = 15,
                    UnitsOnOrder = 0,
                    ReorderLevel = 10,
                    Discontinued = false
                },
                new Product
                {
                    ProductID = 8,
                    ProductName = "Northwoods Cranberry Sauce",
                    SupplierID = 3,
                    CategoryID = 2,
                    QuantityPerUnit = "12 - 12 oz jars",
                    UnitPrice = 40.00m,
                    UnitsInStock = 6,
                    UnitsOnOrder = 0,
                    ReorderLevel = 0,
                    Discontinued = false
                },
                new Product
                {
                    ProductID = 9,
                    ProductName = "Mishi Kobe Niku",
                    SupplierID = 4,
                    CategoryID = 6,
                    QuantityPerUnit = "18 - 500 g pkgs.",
                    UnitPrice = 97.00m,
                    UnitsInStock = 29,
                    UnitsOnOrder = 0,
                    ReorderLevel = 0,
                    Discontinued = true
                },
                new Product
                {
                    ProductID = 10,
                    ProductName = "Ikura",
                    SupplierID = 4,
                    CategoryID = 8,
                    QuantityPerUnit = "12 - 200 ml jars",
                    UnitPrice = 31.00m,
                    UnitsInStock = 31,
                    UnitsOnOrder = 0,
                    ReorderLevel = 0,
                    Discontinued = false
                },
                new Product
                {
                    ProductID = 11,
                    ProductName = "Queso Cabrales",
                    SupplierID = 5,
                    CategoryID = 4,
                    QuantityPerUnit = "1 kg pkg.",
                    UnitPrice = 21.00m,
                    UnitsInStock = 22,
                    UnitsOnOrder = 30,
                    ReorderLevel = 30,
                    Discontinued = false
                },
                new Product
                {
                    ProductID = 12,
                    ProductName = "Queso Manchego La Pastora",
                    SupplierID = 5,
                    CategoryID = 4,
                    QuantityPerUnit = "10 - 500 g pkgs.",
                    UnitPrice = 38.00m,
                    UnitsInStock = 86,
                    UnitsOnOrder = 0,
                    ReorderLevel = 0,
                    Discontinued = false
                }
            };
            modelBuilder.Entity<Product>().HasData(products);

            // Seed Shippers
            var shippers = new[]
            {
                new Shipper
                {
                    ShipperID = 1,
                    CompanyName = "Speedy Express",
                    Phone = "(503) 555-9831"
                },
                new Shipper
                {
                    ShipperID = 2,
                    CompanyName = "United Package",
                    Phone = "(503) 555-3199"
                },
                new Shipper
                {
                    ShipperID = 3,
                    CompanyName = "Federal Shipping",
                    Phone = "(503) 555-9931"
                }
            };
            modelBuilder.Entity<Shipper>().HasData(shippers);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
