using ProductCatalog.Domain.Entities;
using System.Threading.Tasks;

namespace ProductCatalog.Domain.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
    }
}
