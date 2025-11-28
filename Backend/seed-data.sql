-- Seed Users (only if not exists)
INSERT INTO "Users" ("UserID", "Username", "PasswordHash", "Email", "Role", "IsActive", "CreatedAt")
SELECT 1, 'admin', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'admin@productcatalog.com', 'Admin', true, '2024-01-01 00:00:00+00'
WHERE NOT EXISTS (SELECT 1 FROM "Users" WHERE "UserID" = 1);

INSERT INTO "Users" ("UserID", "Username", "PasswordHash", "Email", "Role", "IsActive", "CreatedAt")
SELECT 2, 'user', 'JQZSfMJBQvvgC5GfBl0dw6x68B6bT9O5jj4U8Gs6dA4=', 'user@productcatalog.com', 'User', true, '2024-01-01 00:00:00+00'
WHERE NOT EXISTS (SELECT 1 FROM "Users" WHERE "UserID" = 2);

-- Seed Categories
INSERT INTO "Categories" ("CategoryID", "CategoryName", "Description")
SELECT * FROM (VALUES
    (1, 'Beverages', 'Soft drinks, coffees, teas, beers, and ales'),
    (2, 'Condiments', 'Sweet and savory sauces, relishes, spreads, and seasonings'),
    (3, 'Confections', 'Desserts, candies, and sweet breads'),
    (4, 'Dairy Products', 'Cheeses'),
    (5, 'Grains/Cereals', 'Breads, crackers, pasta, and cereal'),
    (6, 'Meat/Poultry', 'Prepared meats'),
    (7, 'Produce', 'Dried fruit and bean curd'),
    (8, 'Seafood', 'Seaweed and fish')
) AS v("CategoryID", "CategoryName", "Description")
WHERE NOT EXISTS (SELECT 1 FROM "Categories" WHERE "CategoryID" = v."CategoryID");

-- Seed Suppliers
INSERT INTO "Suppliers" ("SupplierID", "CompanyName", "ContactName", "ContactTitle", "Address", "City", "Region", "PostalCode", "Country", "Phone")
SELECT * FROM (VALUES
    (1, 'Exotic Liquids', 'Charlotte Cooper', 'Purchasing Manager', '49 Gilbert St.', 'London', NULL, 'EC1 4SD', 'UK', '(171) 555-2222'),
    (2, 'New Orleans Cajun Delights', 'Shelley Burke', 'Order Administrator', 'P.O. Box 78934', 'New Orleans', 'LA', '70117', 'USA', '(100) 555-4822'),
    (3, 'Grandma Kelly''s Homestead', 'Regina Murphy', 'Sales Representative', '707 Oxford Rd.', 'Ann Arbor', 'MI', '48104', 'USA', '(313) 555-5735'),
    (4, 'Tokyo Traders', 'Yoshi Nagase', 'Marketing Manager', '9-8 Sekimai Musashino-shi', 'Tokyo', NULL, '100', 'Japan', '(03) 3555-5011'),
    (5, 'Cooperativa de Quesos ''Las Cabras''', 'Antonio del Valle Saavedra', 'Export Administrator', 'Calle del Rosal 4', 'Oviedo', 'Asturias', '33007', 'Spain', '(98) 598 76 54')
) AS v("SupplierID", "CompanyName", "ContactName", "ContactTitle", "Address", "City", "Region", "PostalCode", "Country", "Phone")
WHERE NOT EXISTS (SELECT 1 FROM "Suppliers" WHERE "SupplierID" = v."SupplierID");

-- Seed Products
INSERT INTO "Products" ("ProductID", "ProductName", "SupplierID", "CategoryID", "QuantityPerUnit", "UnitPrice", "UnitsInStock", "UnitsOnOrder", "ReorderLevel", "Discontinued")
SELECT * FROM (VALUES
    (1, 'Chai', 1, 1, '10 boxes x 20 bags', 18.00, 39, 0, 10, false),
    (2, 'Chang', 1, 1, '24 - 12 oz bottles', 19.00, 17, 40, 25, false),
    (3, 'Aniseed Syrup', 1, 2, '12 - 550 ml bottles', 10.00, 13, 70, 25, false),
    (4, 'Chef Anton''s Cajun Seasoning', 2, 2, '48 - 6 oz jars', 22.00, 53, 0, 0, false),
    (5, 'Chef Anton''s Gumbo Mix', 2, 2, '36 boxes', 21.35, 0, 0, 0, true),
    (6, 'Grandma''s Boysenberry Spread', 3, 2, '12 - 8 oz jars', 25.00, 120, 0, 25, false),
    (7, 'Uncle Bob''s Organic Dried Pears', 3, 7, '12 - 1 lb pkgs.', 30.00, 15, 0, 10, false),
    (8, 'Northwoods Cranberry Sauce', 3, 2, '12 - 12 oz jars', 40.00, 6, 0, 0, false),
    (9, 'Mishi Kobe Niku', 4, 6, '18 - 500 g pkgs.', 97.00, 29, 0, 0, true),
    (10, 'Ikura', 4, 8, '12 - 200 ml jars', 31.00, 31, 0, 0, false),
    (11, 'Queso Cabrales', 5, 4, '1 kg pkg.', 21.00, 22, 30, 30, false),
    (12, 'Queso Manchego La Pastora', 5, 4, '10 - 500 g pkgs.', 38.00, 86, 0, 0, false)
) AS v("ProductID", "ProductName", "SupplierID", "CategoryID", "QuantityPerUnit", "UnitPrice", "UnitsInStock", "UnitsOnOrder", "ReorderLevel", "Discontinued")
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "ProductID" = v."ProductID");

-- Seed Shippers
INSERT INTO "Shippers" ("ShipperID", "CompanyName", "Phone")
SELECT * FROM (VALUES
    (1, 'Speedy Express', '(503) 555-9831'),
    (2, 'United Package', '(503) 555-3199'),
    (3, 'Federal Shipping', '(503) 555-9931')
) AS v("ShipperID", "CompanyName", "Phone")
WHERE NOT EXISTS (SELECT 1 FROM "Shippers" WHERE "ShipperID" = v."ShipperID");

-- Update sequences to avoid conflicts
SELECT setval('"Categories_CategoryID_seq"', (SELECT MAX("CategoryID") FROM "Categories"));
SELECT setval('"Suppliers_SupplierID_seq"', (SELECT MAX("SupplierID") FROM "Suppliers"));
SELECT setval('"Products_ProductID_seq"', (SELECT MAX("ProductID") FROM "Products"));
SELECT setval('"Shippers_ShipperID_seq"', (SELECT MAX("ShipperID") FROM "Shippers"));
SELECT setval('"Users_UserID_seq"', (SELECT MAX("UserID") FROM "Users"));
