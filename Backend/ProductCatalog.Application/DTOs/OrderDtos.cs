using System;
using System.Collections.Generic;

namespace ProductCatalog.Application.DTOs
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public string? CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public int? EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipCountry { get; set; }
        public List<OrderDetailDto> Details { get; set; } = new List<OrderDetailDto>();
    }

    public class OrderDetailDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public decimal Subtotal => (UnitPrice * Quantity) * (decimal)(1 - Discount);
    }

    public class CreateOrderDto
    {
        public string? CustomerID { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime? RequiredDate { get; set; }
        public int? ShipVia { get; set; }
        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipCountry { get; set; }
        
        public List<CreateOrderDetailDto> Details { get; set; } = new List<CreateOrderDetailDto>();
    }

    public class CreateOrderDetailDto
    {
        public int ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
    }
}
