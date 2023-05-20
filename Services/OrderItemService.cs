using Jordnaer.Interfaces;
using Jordnaer.Models;
using System.Data.SqlClient;

namespace Jordnaer.Services
{
    public class OrderItemService : Connection, IOrderItemService
    {








        public OrderItemService(IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<bool> AddOrderItemsAsync(int orderId, List<OrderItem> orderItems)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    foreach (var orderItem in orderItems)
                    {
                        // Insert order item into the database
                        using (SqlCommand command = new SqlCommand(insertOrderItemSQL, connection))
                        {
                            command.Parameters.AddWithValue("@OrderId", orderId);
                            command.Parameters.AddWithValue("@ItemId", orderItem.ItemID);
                            command.Parameters.AddWithValue("@Quantity", orderItem.Quantity);

                            int rowsAffected = await command.ExecuteNonQueryAsync();

                            if (rowsAffected <= 0)
                                return false;
                        }
                    }

                    return true;
                }
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine("Database error: " + sqlex.Message);
                // Handle database error
            }
            catch (Exception ex)
            {
                Console.WriteLine("General error: " + ex.Message);
                // Handle general error
            }

            return false; // Insert failed
        }


        public Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveOrderItemAsync(int orderItemId)
        {
            throw new NotImplementedException();
        }
    }
}
