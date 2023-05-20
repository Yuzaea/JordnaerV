using Jordnaer.Interfaces;
using Jordnaer.Models;
using System.Data.SqlClient;

namespace Jordnaer.Services
{
    public class OrderService : Connection, IOrderService
    {

        private string insertOrderSQL = "INSERT INTO Orders (MemberID, OrderDate, TotalPrice) VALUES (@MemberId, @OrderDate, @TotalPrice)";
        private string getPriceSQL = "SELECT Item_Price FROM Item WHERE Item_ID = @ItemId";




        private readonly IOrderItemService orderItemService;

        public OrderService(IConfiguration configuration, IOrderItemService orderItemService) : base(configuration)
        {
            this.orderItemService = orderItemService;
        }

        public OrderService(IConfiguration configuration) : base(configuration)
        {
        }




        public Task<bool> CancelOrderAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<Orders> GetOrderByIdAsync(int orderId)
        {
            throw new NotImplementedException();
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



        public Task<bool> UpdateOrderAsync(Orders order, int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
