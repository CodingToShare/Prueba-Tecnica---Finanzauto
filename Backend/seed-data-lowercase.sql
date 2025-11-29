-- Seed data with lowercase column names

-- Create users table if not exists (for authentication)
CREATE TABLE IF NOT EXISTS users (
    userid SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL,
    passwordhash VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL,
    createdat TIMESTAMP NOT NULL DEFAULT NOW(),
    isactive BOOLEAN NOT NULL DEFAULT true
);

CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);
CREATE INDEX IF NOT EXISTS idx_users_email ON users(email);

-- Seed Users (passwords are hashed with BCrypt)
-- admin:Admin123! / user:User123!
INSERT INTO users (userid, username, email, passwordhash, role, createdat, isactive)
SELECT * FROM (VALUES
    (1, 'admin', 'admin@finanzauto.com', '$2a$11$oZ5rpmG06zQkRP8qHjfwguMzf6KMoMlxhshPRwtg3yK6i2TXy3..K', 'Admin', NOW(), true),
    (2, 'user', 'user@finanzauto.com', '$2a$11$25C29iBrHq7b6M2di/VYPu/najFklgqimvqYhL1NiJ8enC8FGN33W', 'User', NOW(), true)
) AS v(userid, username, email, passwordhash, role, createdat, isactive)
WHERE NOT EXISTS (SELECT 1 FROM users WHERE userid = v.userid);

-- Seed Categories
INSERT INTO categories (categoryid, categoryname, description)
SELECT * FROM (VALUES
    (1, 'Beverages', 'Soft drinks, coffees, teas, beers, and ales'),
    (2, 'Condiments', 'Sweet and savory sauces, relishes, spreads, and seasonings'),
    (3, 'Confections', 'Desserts, candies, and sweet breads'),
    (4, 'Dairy Products', 'Cheeses'),
    (5, 'Grains/Cereals', 'Breads, crackers, pasta, and cereal'),
    (6, 'Meat/Poultry', 'Prepared meats'),
    (7, 'Produce', 'Dried fruit and bean curd'),
    (8, 'Seafood', 'Seaweed and fish')
) AS v(categoryid, categoryname, description)
WHERE NOT EXISTS (SELECT 1 FROM categories WHERE categoryid = v.categoryid);

-- Seed Suppliers
INSERT INTO suppliers (supplierid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone)
SELECT * FROM (VALUES
    (1, 'Exotic Liquids', 'Charlotte Cooper', 'Purchasing Manager', '49 Gilbert St.', 'London', NULL, 'EC1 4SD', 'UK', '(171) 555-2222'),
    (2, 'New Orleans Cajun Delights', 'Shelley Burke', 'Order Administrator', 'P.O. Box 78934', 'New Orleans', 'LA', '70117', 'USA', '(100) 555-4822'),
    (3, 'Grandma Kelly''s Homestead', 'Regina Murphy', 'Sales Representative', '707 Oxford Rd.', 'Ann Arbor', 'MI', '48104', 'USA', '(313) 555-5735'),
    (4, 'Tokyo Traders', 'Yoshi Nagase', 'Marketing Manager', '9-8 Sekimai Musashino-shi', 'Tokyo', NULL, '100', 'Japan', '(03) 3555-5011'),
    (5, 'Cooperativa de Quesos ''Las Cabras''', 'Antonio del Valle Saavedra', 'Export Administrator', 'Calle del Rosal 4', 'Oviedo', 'Asturias', '33007', 'Spain', '(98) 598 76 54')
) AS v(supplierid, companyname, contactname, contacttitle, address, city, region, postalcode, country, phone)
WHERE NOT EXISTS (SELECT 1 FROM suppliers WHERE supplierid = v.supplierid);

-- Seed Products
INSERT INTO products (productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued)
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
) AS v(productid, productname, supplierid, categoryid, quantityperunit, unitprice, unitsinstock, unitsonorder, reorderlevel, discontinued)
WHERE NOT EXISTS (SELECT 1 FROM products WHERE productid = v.productid);

-- Seed Shippers
INSERT INTO shippers (shipperid, companyname, phone)
SELECT * FROM (VALUES
    (1, 'Speedy Express', '(503) 555-9831'),
    (2, 'United Package', '(503) 555-3199'),
    (3, 'Federal Shipping', '(503) 555-9931')
) AS v(shipperid, companyname, phone)
WHERE NOT EXISTS (SELECT 1 FROM shippers WHERE shipperid = v.shipperid);

-- Reset sequences
SELECT setval('users_userid_seq', (SELECT MAX(userid) FROM users));
SELECT setval('categories_categoryid_seq', (SELECT MAX(categoryid) FROM categories));
SELECT setval('suppliers_supplierid_seq', (SELECT MAX(supplierid) FROM suppliers));
SELECT setval('products_productid_seq', (SELECT MAX(productid) FROM products));
SELECT setval('shippers_shipperid_seq', (SELECT MAX(shipperid) FROM shippers));
