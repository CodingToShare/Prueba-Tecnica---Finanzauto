using System;

namespace ProductCatalog.Application.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class CreateEmployeeDto
    {
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? Title { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? ReportsTo { get; set; }
    }
}
