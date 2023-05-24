using Jordnaer.Models;

namespace Jordnaer.Interfaces
{
    public interface IOrderItemService
    {
        Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);

        Task<bool> AddOrderItemsAsync(int orderId, List<OrderItem> orderItems);

        //Task<bool> RemoveOrderItemAsync(int orderItemId);
    }
}
