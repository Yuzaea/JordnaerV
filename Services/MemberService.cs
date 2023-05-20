using Jordnaer.Interfaces;
using Jordnaer.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Jordnaer.Services
{
    public class MemberService : Connection, IMemberService
    {
        private string deleteSQL = "DELETE FROM Members WHERE Member_ID = @MemberId";
        private string insertSQL = "INSERT INTO Members (Name, Email, Password) VALUES (@Name, @Email, @Password)";
        private string HighestID = "SELECT MAX(Member_ID) FROM Members";
        private string MemberBYID = "SELECT MemberID, Name, Email, Password FROM Members WHERE MemberID = @MemberID";
        private string UpdateMemberSQL = "UPDATE Member SET Name = @Name, Email = @Email, Password = @Password WHERE MemberID = @MemberID";

        private string GetMemberOrders = "SELECT OrderID, OrderDate, TotalPrice, MemberID FROM Orders WHERE MemberID = @MemberID";
        public MemberService(IConfiguration configuration) : base(configuration)
        {
        }


        public async Task<bool> CreateMemberAsync(Member member)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Create a new SqlCommand with the query and connection
                    using (SqlCommand command = new SqlCommand(insertSQL, connection))
                    {
                        // Set the parameter values
                        command.Parameters.AddWithValue("@Name", member.Name);
                        command.Parameters.AddWithValue("@Email", member.Email);
                        command.Parameters.AddWithValue("@Password", member.Password);

                        // Execute the query
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        // Return true if at least one row was affected (insert successful)
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

            return false; // Insert failed
        }


        public async Task<bool> DeleteMemberAsync(int memberId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(deleteSQL, connection))
                    {
                        // Set the parameter value
                        command.Parameters.AddWithValue("@MemberId", memberId);

                        // Execute the query
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        // Return true if at least one row was affected (deletion successful)
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

            return false; // Deletion failed
        }


        public async Task<int> FindHighestMemberIdAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Create a new SqlCommand with the query and connection
                    using (SqlCommand command = new SqlCommand(HighestID, connection))
                    {
                        // Execute the query and retrieve the result
                        object result = await command.ExecuteScalarAsync();

                        // Check if the result is null and convert it to an int
                        if (result != null && int.TryParse(result.ToString(), out int highestMemberId))
                        {
                            return highestMemberId;
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

            return 0; // Return 0 if the highest member ID was not found or an error occurred
        }



        public async Task<Member> GetMemberByIdAsync(int memberId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(MemberBYID, connection))
                    {
                        command.Parameters.AddWithValue("@MemberID", memberId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                Member member = new Member
                                {
                                    MemberID = reader.GetInt32(reader.GetOrdinal("MemberID")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Password = reader.GetString(reader.GetOrdinal("Password"))
                                };

                                return member;
                            }
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

            return null; // Member not found or error occurred
        }

        public async Task<List<Orders>> GetMemberOrdersAsync(int memberId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(GetMemberOrders, connection))
                    {
                        command.Parameters.AddWithValue("@MemberID", memberId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            List<Orders> orders = new List<Orders>();

                            while (reader.Read())
                            {
                                Orders order = new Orders
                                {
                                    OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                                    OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                    TotalPrice = reader.GetFloat(reader.GetOrdinal("TotalPrice")),
                                    MemberID = reader.GetInt32(reader.GetOrdinal("MemberID"))
                                };

                                orders.Add(order);
                            }

                            return orders;
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

            return null; // Member orders not found or error occurred
        }

        public async Task<bool> UpdateMemberAsync(Member member, int memberId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(UpdateMemberSQL, connection))
                    {
                        command.Parameters.AddWithValue("@Name", member.Name);
                        command.Parameters.AddWithValue("@Email", member.Email);
                        command.Parameters.AddWithValue("@Password", member.Password);
                        command.Parameters.AddWithValue("@MemberID", memberId);

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0; // Return true if the member was updated successfully
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

            return false; // Member update failed or error occurred
        }
    }
}
