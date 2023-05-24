using Jordnaer.Interfaces;
using Jordnaer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Jordnaer.Services
{
    public class OrderService : Connection, IOrderService
    {

        private string insertOrderSQL = "INSERT INTO Orders (MemberID, OrderDate, TotalPrice) VALUES (@MemberID, @OrderDate, @TotalPrice)";
        private string getPriceSQL = "SELECT Item_Price FROM Item WHERE Item_ID = @ItemID";
        private string UpdateSQL = "UPDATE Orders SET OrderDate = @OrderDate, TotalPrice = @TotalPrice WHERE OrderID = @OrderID";
        private string getOrderByIdSQL = "SELECT Order_ID, Order_Date, Total_Price, Member_ID FROM Orders WHERE Order_ID = @OrderID";
        private string deleteOrderSQL = "DELETE FROM Orders WHERE OrderID = @OrderID";


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

                    using (SqlCommand checkCommand = new SqlCommand(getOrderByIdSQL, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@OrderID", orderId);

                        using (SqlDataReader reader = await checkCommand.ExecuteReaderAsync())
                        {
                            if (!reader.HasRows)
                            {
                                return false;
                            }
                        }
                    }

                    using (SqlCommand command = new SqlCommand(deleteOrderSQL, connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", orderId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0;
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

            return false; // Delete operation failed
        }


        public async Task<Orders> GetOrderByIdAsync(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(getOrderByIdSQL, connection))
                {
                    command.Parameters.AddWithValue("@OrderID", orderId);

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
            double totalPrice = 0; // Use double instead of float

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var orderItem in orderItems)
                {
                    using (SqlCommand command = new SqlCommand(getPriceSQL, connection))
                    {
                        command.Parameters.AddWithValue("@ItemID", orderItem.ItemID);
                        double itemPrice = Convert.ToDouble(command.ExecuteScalar()); // Convert to double

                        // Calculate the subtotal for each order item and accumulate the total price
                        double subtotal = orderItem.Quantity * itemPrice; // Use double for intermediate calculations
                        totalPrice += subtotal;
                    }
                }
            }

            return (float)totalPrice; // Cast the final total price back to float
        }


        public async Task<int> CreateOrderAsync(int memberId, List<OrderItem> orderItems)
        {
            int orderId = 0;

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

                    // Insert the order into the database and retrieve the generated OrderID
                    using (SqlCommand command = new SqlCommand(insertOrderSQL + "; SELECT CAST(SCOPE_IDENTITY() AS INT)", connection))
                    {
                        command.Parameters.AddWithValue("@MemberID", order.MemberID);
                        command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);

                        object result = await command.ExecuteScalarAsync();
                        if (result != null && int.TryParse(result.ToString(), out orderId))
                        {
                            return orderId;
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

            return 0;
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

                        return rowsAffected > 0; 
                    }
                }
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine("Database error: " + sqlex.Message);

            }
            catch (Exception ex)
            {
                Console.WriteLine("General error: " + ex.Message);

            }

            return false; 
        }
    }
}
