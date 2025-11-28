using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        // Métodos para jerarquía de empleados si fuera necesario
    }
}
