using ECommerceApp.Domain.Entities;

namespace ECommerceApp.Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> PlaceOrderAsync(int customerId, List<OrderItem> orderItems);
        Task<List<Order>> GetUserOrdersAsync(int userId);
        Task DeleteOrderAsync(int id);
    }
}
