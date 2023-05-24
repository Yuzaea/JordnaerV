using Jordnaer.Interfaces;
using Jordnaer.Models;
using System.Data.SqlClient;

namespace Jordnaer.Services
{
    public class OrderItemService : Connection, IOrderItemService
    {


        private string insertOrderItemSQL = "INSERT INTO OrderItemsss (OrderID, Item_ID, Quantity) VALUES (@OrderID, @Item_ID, @Quantity)";
        private string query = "SELECT * FROM OrderItemsss WHERE OrderID = @OrderID";





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
                            command.Parameters.AddWithValue("@OrderID", orderId);
                            command.Parameters.AddWithValue("@Item_ID", orderItem.ItemID);
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


        public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();


                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);

                        List<OrderItem> orderItems = new List<OrderItem>();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                OrderItem orderItem = new OrderItem(
                                    (int)reader["OrderID"],
                                    (int)reader["ItemID"],
                                    (int)reader["Quantity"]
                                );



                                orderItems.Add(orderItem);
                            }
                        }

                        return orderItems;
                    }
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

            return null; // Failed to retrieve order items
        }


        //public async Task<bool> RemoveOrderItemAsync(int orderItemId)
        //{
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            await connection.OpenAsync();

        //            Delete the order item from the database
        //            using (SqlCommand command = new SqlCommand(deleteOrderItemSQL, connection))
        //            {
        //                command.Parameters.AddWithValue("@OrderItemId", orderItemId);

        //                int rowsAffected = await command.ExecuteNonQueryAsync();

        //                return rowsAffected > 0;
        //            }
        //        }
        //    }
        //    catch (SqlException sqlex)
        //    {
        //        Console.WriteLine("Database error: " + sqlex.Message);
        //        Handle database error
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("General error: " + ex.Message);
        //        Handle general error
        //    }

        //    return false; // Deletion failed
        //}

    }
}
