using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace ProductCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Users
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Username", "PasswordHash", "Email", "Role", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    { 1, "admin", "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", "admin@productcatalog.com", "Admin", true, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "user", "JQZSfMJBQvvgC5GfBl0dw6x68B6bT9O5jj4U8Gs6dA4=", "user@productcatalog.com", "User", true, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
                });

            // Seed Categories
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "CategoryName", "Description", "Picture" },
                values: new object[,]
                {
                    { 1, "Beverages", "Soft drinks, coffees, teas, beers, and ales", null },
                    { 2, "Condiments", "Sweet and savory sauces, relishes, spreads, and seasonings", null },
                    { 3, "Confections", "Desserts, candies, and sweet breads", null },
                    { 4, "Dairy Products", "Cheeses", null },
                    { 5, "Grains/Cereals", "Breads, crackers, pasta, and cereal", null },
                    { 6, "Meat/Poultry", "Prepared meats", null },
                    { 7, "Produce", "Dried fruit and bean curd", null },
                    { 8, "Seafood", "Seaweed and fish", null }
                });

            // Seed Suppliers
            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "SupplierID", "CompanyName", "ContactName", "ContactTitle", "Address", "City", "Region", "PostalCode", "Country", "Phone", "Fax", "HomePage" },
                values: new object[,]
                {
                    { 1, "Exotic Liquids", "Charlotte Cooper", "Purchasing Manager", "49 Gilbert St.", "London", null, "EC1 4SD", "UK", "(171) 555-2222", null, null },
                    { 2, "New Orleans Cajun Delights", "Shelley Burke", "Order Administrator", "P.O. Box 78934", "New Orleans", "LA", "70117", "USA", "(100) 555-4822", null, null },
                    { 3, "Grandma Kelly's Homestead", "Regina Murphy", "Sales Representative", "707 Oxford Rd.", "Ann Arbor", "MI", "48104", "USA", "(313) 555-5735", null, null },
                    { 4, "Tokyo Traders", "Yoshi Nagase", "Marketing Manager", "9-8 Sekimai Musashino-shi", "Tokyo", null, "100", "Japan", "(03) 3555-5011", null, null },
                    { 5, "Cooperativa de Quesos 'Las Cabras'", "Antonio del Valle Saavedra", "Export Administrator", "Calle del Rosal 4", "Oviedo", "Asturias", "33007", "Spain", "(98) 598 76 54", null, null }
                });

            // Seed Products
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductID", "ProductName", "SupplierID", "CategoryID", "QuantityPerUnit", "UnitPrice", "UnitsInStock", "UnitsOnOrder", "ReorderLevel", "Discontinued" },
                values: new object[,]
                {
                    { 1, "Chai", 1, 1, "10 boxes x 20 bags", 18.00m, 39, 0, 10, false },
                    { 2, "Chang", 1, 1, "24 - 12 oz bottles", 19.00m, 17, 40, 25, false },
                    { 3, "Aniseed Syrup", 1, 2, "12 - 550 ml bottles", 10.00m, 13, 70, 25, false },
                    { 4, "Chef Anton's Cajun Seasoning", 2, 2, "48 - 6 oz jars", 22.00m, 53, 0, 0, false },
                    { 5, "Chef Anton's Gumbo Mix", 2, 2, "36 boxes", 21.35m, 0, 0, 0, true },
                    { 6, "Grandma's Boysenberry Spread", 3, 2, "12 - 8 oz jars", 25.00m, 120, 0, 25, false },
                    { 7, "Uncle Bob's Organic Dried Pears", 3, 7, "12 - 1 lb pkgs.", 30.00m, 15, 0, 10, false },
                    { 8, "Northwoods Cranberry Sauce", 3, 2, "12 - 12 oz jars", 40.00m, 6, 0, 0, false },
                    { 9, "Mishi Kobe Niku", 4, 6, "18 - 500 g pkgs.", 97.00m, 29, 0, 0, true },
                    { 10, "Ikura", 4, 8, "12 - 200 ml jars", 31.00m, 31, 0, 0, false },
                    { 11, "Queso Cabrales", 5, 4, "1 kg pkg.", 21.00m, 22, 30, 30, false },
                    { 12, "Queso Manchego La Pastora", 5, 4, "10 - 500 g pkgs.", 38.00m, 86, 0, 0, false }
                });

            // Seed Shippers
            migrationBuilder.InsertData(
                table: "Shippers",
                columns: new[] { "ShipperID", "CompanyName", "Phone" },
                values: new object[,]
                {
                    { 1, "Speedy Express", "(503) 555-9831" },
                    { 2, "United Package", "(503) 555-3199" },
                    { 3, "Federal Shipping", "(503) 555-9931" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete seeded data in reverse order (respecting foreign keys)
            migrationBuilder.DeleteData(table: "Products", keyColumn: "ProductID", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
            migrationBuilder.DeleteData(table: "Shippers", keyColumn: "ShipperID", keyValues: new object[] { 1, 2, 3 });
            migrationBuilder.DeleteData(table: "Suppliers", keyColumn: "SupplierID", keyValues: new object[] { 1, 2, 3, 4, 5 });
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "CategoryID", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8 });
            migrationBuilder.DeleteData(table: "Users", keyColumn: "UserID", keyValues: new object[] { 1, 2 });
        }
    }
}
