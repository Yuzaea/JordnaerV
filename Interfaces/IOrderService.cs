using Jordnaer.Models;

namespace Jordnaer.Interfaces
{
    public interface IOrderService
    {

        Task<Orders> GetOrderByIdAsync(int orderId);

        Task<bool> CreateOrderAsync(int memberId, List<OrderItem> orderItems);

        Task<bool> CancelOrderAsync(int orderId);

        Task<bool> UpdateOrderAsync(Orders order, int orderId);


    }
}
