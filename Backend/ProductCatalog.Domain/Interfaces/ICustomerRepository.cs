using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        // Futuros métodos de búsqueda por región, ciudad, etc.
    }
}
