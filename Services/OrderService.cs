using Jordnaer.Interfaces;
using Jordnaer.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Jordnaer.Services
{
    public class OrderService : Connection, IOrderService
    {

        private string insertOrderSQL = "INSERT INTO Orders (MemberID, OrderDate, TotalPrice) VALUES (@MemberId, @OrderDate, @TotalPrice)";
        private string getPriceSQL = "SELECT Item_Price FROM Item WHERE Item_ID = @ItemId";
        private string UpdateSQL = "UPDATE Orders SET OrderDate = @OrderDate, TotalPrice = @TotalPrice WHERE OrderID = @OrderID";
        private string getOrderByIdSQL = "SELECT Order_ID, Order_Date, Total_Price, Member_ID FROM Orders WHERE Order_ID = @OrderId";
        private string cancelOrderSQL = "UPDATE Orders SET OrderStatus = 'Cancelled'WHERE OrderID = @OrderId";
        private string deleteOrderSQL = "DELETE FROM Orders WHERE OrderID = @OrderId";






        private readonly IOrderItemService orderItemService;

        public OrderService(IConfiguration configuration, IOrderItemService orderItemService) : base(configuration)
        {
            this.orderItemService = orderItemService;
        }

        public OrderService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(deleteOrderSQL, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0; // Return true if rows were affected (delete successful)
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

            return false; // Delete failed
        }



        public async Task<bool> CancelOrderAsync(int orderId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Check if the order exists
                    using (SqlCommand checkCommand = new SqlCommand(getOrderByIdSQL, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@OrderId", orderId);

                        using (SqlDataReader reader = await checkCommand.ExecuteReaderAsync())
                        {
                            if (!reader.HasRows)
                            {
                                // Order with the specified ID was not found
                                return false;
                            }
                        }
                    }

                    // Update the order's status to cancelled
                    using (SqlCommand command = new SqlCommand(cancelOrderSQL, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0; // Return true if rows were affected (update successful)
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

            return false; // Cancel operation failed
        }


        public async Task<Orders> GetOrderByIdAsync(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(getOrderByIdSQL, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Orders order = new Orders
                            {
                                OrderID = Convert.ToInt32(reader["Order_ID"]),
                                OrderDate = Convert.ToDateTime(reader["Order_Date"]),
                                TotalPrice = Convert.ToSingle(reader["Total_Price"]),
                                MemberID = Convert.ToInt32(reader["Member_ID"]),
                            };

                            return order;
                        }
                    }
                }
            }

            return null; // Order not found
        }

        public float CalculateTotalPrice(List<OrderItem> orderItems)
        {
            float totalPrice = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var orderItem in orderItems)
                {

                    using (SqlCommand command = new SqlCommand(getPriceSQL, connection))
                    {
                        command.Parameters.AddWithValue("@ItemId", orderItem.ItemID);
                        float itemPrice = (float)command.ExecuteScalar();

                        // Calculate the subtotal for each order item and accumulate the total price
                        float subtotal = orderItem.Quantity * itemPrice;
                        totalPrice += subtotal;
                    }
                }
            }

            return totalPrice;
        }

        public async Task<bool> CreateOrderAsync(int memberId, List<OrderItem> orderItems)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Create a new order instance
                    Orders order = new Orders
                    {
                        MemberID = memberId,
                        OrderDate = DateTime.Now,
                        TotalPrice = CalculateTotalPrice(orderItems),
                    };

                    // Insert the order into the database
                    using (SqlCommand command = new SqlCommand(insertOrderSQL, connection))
                    {
                        command.Parameters.AddWithValue("@MemberId", order.MemberID);
                        command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            // Retrieve the generated OrderId from the inserted row
                            command.CommandText = "SELECT SCOPE_IDENTITY()";
                            int orderId = Convert.ToInt32(await command.ExecuteScalarAsync());

                            // Add order items to the database
                            await orderItemService.AddOrderItemsAsync(orderId, orderItems);

                            return true;
                        }
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

            return false; // Insert failed
        }

        public async Task<bool> UpdateOrderAsync(Orders order, int orderId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(UpdateSQL, connection))
                    {
                        command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                        command.Parameters.AddWithValue("@OrderID", orderId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0; // Return true if the order was updated successfully
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

            return false; // Order update failed or error occurred
        }
    }
}
