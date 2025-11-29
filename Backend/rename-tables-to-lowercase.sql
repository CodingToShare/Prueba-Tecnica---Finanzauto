-- Script para renombrar todas las tablas y columnas a minúsculas para PostgreSQL
-- Este script debe ejecutarse después de aplicar las migraciones iniciales

-- Renombrar tablas (ignora errores si ya están en minúsculas)
ALTER TABLE "Categories" RENAME TO categories;
ALTER TABLE "Products" RENAME TO products;
ALTER TABLE "Suppliers" RENAME TO suppliers;
ALTER TABLE "Customers" RENAME TO customers;
ALTER TABLE "Employees" RENAME TO employees;
ALTER TABLE "Orders" RENAME TO orders;
ALTER TABLE "OrderDetails" RENAME TO orderdetails;
ALTER TABLE "Shippers" RENAME TO shippers;

-- Categories
ALTER TABLE categories RENAME COLUMN "CategoryID" TO categoryid;
ALTER TABLE categories RENAME COLUMN "CategoryName" TO categoryname;
ALTER TABLE categories RENAME COLUMN "Description" TO description;
ALTER TABLE categories RENAME COLUMN "Picture" TO picture;

-- Suppliers
ALTER TABLE suppliers RENAME COLUMN "SupplierID" TO supplierid;
ALTER TABLE suppliers RENAME COLUMN "CompanyName" TO companyname;
ALTER TABLE suppliers RENAME COLUMN "ContactName" TO contactname;
ALTER TABLE suppliers RENAME COLUMN "ContactTitle" TO contacttitle;
ALTER TABLE suppliers RENAME COLUMN "Address" TO address;
ALTER TABLE suppliers RENAME COLUMN "City" TO city;
ALTER TABLE suppliers RENAME COLUMN "Region" TO region;
ALTER TABLE suppliers RENAME COLUMN "PostalCode" TO postalcode;
ALTER TABLE suppliers RENAME COLUMN "Country" TO country;
ALTER TABLE suppliers RENAME COLUMN "Phone" TO phone;
ALTER TABLE suppliers RENAME COLUMN "Fax" TO fax;
ALTER TABLE suppliers RENAME COLUMN "HomePage" TO homepage;

-- Products
ALTER TABLE products RENAME COLUMN "ProductID" TO productid;
ALTER TABLE products RENAME COLUMN "ProductName" TO productname;
ALTER TABLE products RENAME COLUMN "SupplierID" TO supplierid;
ALTER TABLE products RENAME COLUMN "CategoryID" TO categoryid;
ALTER TABLE products RENAME COLUMN "QuantityPerUnit" TO quantityperunit;
ALTER TABLE products RENAME COLUMN "UnitPrice" TO unitprice;
ALTER TABLE products RENAME COLUMN "UnitsInStock" TO unitsinstock;
ALTER TABLE products RENAME COLUMN "UnitsOnOrder" TO unitsonorder;
ALTER TABLE products RENAME COLUMN "ReorderLevel" TO reorderlevel;
ALTER TABLE products RENAME COLUMN "Discontinued" TO discontinued;

-- Shippers
ALTER TABLE shippers RENAME COLUMN "ShipperID" TO shipperid;
ALTER TABLE shippers RENAME COLUMN "CompanyName" TO companyname;
ALTER TABLE shippers RENAME COLUMN "Phone" TO phone;

-- Customers
ALTER TABLE customers RENAME COLUMN "CustomerID" TO customerid;
ALTER TABLE customers RENAME COLUMN "CompanyName" TO companyname;
ALTER TABLE customers RENAME COLUMN "ContactName" TO contactname;
ALTER TABLE customers RENAME COLUMN "ContactTitle" TO contacttitle;
ALTER TABLE customers RENAME COLUMN "Address" TO address;
ALTER TABLE customers RENAME COLUMN "City" TO city;
ALTER TABLE customers RENAME COLUMN "Region" TO region;
ALTER TABLE customers RENAME COLUMN "PostalCode" TO postalcode;
ALTER TABLE customers RENAME COLUMN "Country" TO country;
ALTER TABLE customers RENAME COLUMN "Phone" TO phone;
ALTER TABLE customers RENAME COLUMN "Fax" TO fax;

-- Employees
ALTER TABLE employees RENAME COLUMN "EmployeeID" TO employeeid;
ALTER TABLE employees RENAME COLUMN "LastName" TO lastname;
ALTER TABLE employees RENAME COLUMN "FirstName" TO firstname;
ALTER TABLE employees RENAME COLUMN "Title" TO title;
ALTER TABLE employees RENAME COLUMN "TitleOfCourtesy" TO titleofcourtesy;
ALTER TABLE employees RENAME COLUMN "BirthDate" TO birthdate;
ALTER TABLE employees RENAME COLUMN "HireDate" TO hiredate;
ALTER TABLE employees RENAME COLUMN "Address" TO address;
ALTER TABLE employees RENAME COLUMN "City" TO city;
ALTER TABLE employees RENAME COLUMN "Region" TO region;
ALTER TABLE employees RENAME COLUMN "PostalCode" TO postalcode;
ALTER TABLE employees RENAME COLUMN "Country" TO country;
ALTER TABLE employees RENAME COLUMN "HomePhone" TO homephone;
ALTER TABLE employees RENAME COLUMN "Extension" TO extension;
ALTER TABLE employees RENAME COLUMN "Photo" TO photo;
ALTER TABLE employees RENAME COLUMN "Notes" TO notes;
ALTER TABLE employees RENAME COLUMN "ReportsTo" TO reportsto;
ALTER TABLE employees RENAME COLUMN "PhotoPath" TO photopath;

-- Orders
ALTER TABLE orders RENAME COLUMN "OrderID" TO orderid;
ALTER TABLE orders RENAME COLUMN "CustomerID" TO customerid;
ALTER TABLE orders RENAME COLUMN "EmployeeID" TO employeeid;
ALTER TABLE orders RENAME COLUMN "OrderDate" TO orderdate;
ALTER TABLE orders RENAME COLUMN "RequiredDate" TO requireddate;
ALTER TABLE orders RENAME COLUMN "ShippedDate" TO shippeddate;
ALTER TABLE orders RENAME COLUMN "ShipVia" TO shipvia;
ALTER TABLE orders RENAME COLUMN "Freight" TO freight;
ALTER TABLE orders RENAME COLUMN "ShipName" TO shipname;
ALTER TABLE orders RENAME COLUMN "ShipAddress" TO shipaddress;
ALTER TABLE orders RENAME COLUMN "ShipCity" TO shipcity;
ALTER TABLE orders RENAME COLUMN "ShipRegion" TO shipregion;
ALTER TABLE orders RENAME COLUMN "ShipPostalCode" TO shippostalcode;
ALTER TABLE orders RENAME COLUMN "ShipCountry" TO shipcountry;

-- OrderDetails
ALTER TABLE orderdetails RENAME COLUMN "OrderID" TO orderid;
ALTER TABLE orderdetails RENAME COLUMN "ProductID" TO productid;
ALTER TABLE orderdetails RENAME COLUMN "UnitPrice" TO unitprice;
ALTER TABLE orderdetails RENAME COLUMN "Quantity" TO quantity;
ALTER TABLE orderdetails RENAME COLUMN "Discount" TO discount;

-- Rename sequences to lowercase
ALTER SEQUENCE "Categories_CategoryID_seq" RENAME TO categories_categoryid_seq;
ALTER SEQUENCE "Products_ProductID_seq" RENAME TO products_productid_seq;
ALTER SEQUENCE "Suppliers_SupplierID_seq" RENAME TO suppliers_supplierid_seq;
ALTER SEQUENCE "Shippers_ShipperID_seq" RENAME TO shippers_shipperid_seq;
ALTER SEQUENCE "Employees_EmployeeID_seq" RENAME TO employees_employeeid_seq;
ALTER SEQUENCE "Orders_OrderID_seq" RENAME TO orders_orderid_seq;

-- Update sequence values (las columnas IDENTITY ya están conectadas automáticamente)
SELECT setval('categories_categoryid_seq', (SELECT COALESCE(MAX(categoryid), 0) + 1 FROM categories), false);
SELECT setval('products_productid_seq', (SELECT COALESCE(MAX(productid), 0) + 1 FROM products), false);
SELECT setval('suppliers_supplierid_seq', (SELECT COALESCE(MAX(supplierid), 0) + 1 FROM suppliers), false);
SELECT setval('shippers_shipperid_seq', (SELECT COALESCE(MAX(shipperid), 0) + 1 FROM shippers), false);
