using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.CategoryID);
            builder.Property(c => c.CategoryName).IsRequired().HasMaxLength(15);
            builder.Property(c => c.Description).HasColumnType("text");
            // Picture en Northwind original es image (byte[]), aquí lo mapeamos como string/text para URL simulada
            builder.Property(c => c.Picture).HasColumnType("text");
        }
    }

    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.ProductID);
            builder.Property(p => p.ProductName).IsRequired().HasMaxLength(40);
            builder.Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
            
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID);

            builder.HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierID);
        }
    }

    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(s => s.SupplierID);
            builder.Property(s => s.CompanyName).IsRequired().HasMaxLength(40);
        }
    }

    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.CustomerID);
            builder.Property(c => c.CustomerID).IsFixedLength().HasMaxLength(5);
            builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(40);
        }
    }

    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.EmployeeID);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(20);
            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(10);
            
            builder.HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ReportsTo);
        }
    }

    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.OrderID);
            builder.Property(o => o.Freight).HasColumnType("decimal(18,2)");
            builder.Property(o => o.CustomerID).IsFixedLength().HasMaxLength(5);

            builder.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerID);

            builder.HasOne(o => o.Employee)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.EmployeeID);

            builder.HasOne(o => o.Shipper)
                .WithMany(o => o.Orders)
                .HasForeignKey(o => o.ShipVia);
        }
    }

    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            // Clave compuesta
            builder.HasKey(od => new { od.OrderID, od.ProductID });

            builder.Property(od => od.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(od => od.Quantity).IsRequired();
            builder.Property(od => od.Discount).HasColumnType("real");

            builder.HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID);

            builder.HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductID);
        }
    }

    public class ShipperConfiguration : IEntityTypeConfiguration<Shipper>
    {
        public void Configure(EntityTypeBuilder<Shipper> builder)
        {
            builder.HasKey(s => s.ShipperID);
            builder.Property(s => s.CompanyName).IsRequired().HasMaxLength(40);
        }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserID);
            builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            builder.Property(u => u.Role).IsRequired().HasMaxLength(20);
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.IsActive).IsRequired();

            // Create unique index on Username and Email
            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}
