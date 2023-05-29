using Jordnaer.Interfaces;
using Jordnaer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Jordnaer.Services
{
    public class MemberService : Connection, IMemberService
    {
        private string deleteSQL = "DELETE FROM Member WHERE MemberID = @MemberID";
        private string insertSQL = "INSERT INTO Member (Name, Email, Password) VALUES (@Name, @Email, @Password)";
        private string HighestID = "SELECT MAX(MemberID) FROM Member";
        private string MemberBYID = "SELECT MemberID, Name, Email, Password FROM Member WHERE MemberID = @MemberID";
        private string UpdateMemberSQL = "UPDATE Member SET Name = @Name, Email = @Email, Password = @Password WHERE MemberID = @MemberID";
        private String queryString = "select * from Member";
        private string GetMemberOrders = "SELECT OrderID, OrderDate, TotalPrice, MemberID FROM Orders WHERE MemberID = @MemberID";
        private string CheckForEmailSQL = "SELECT COUNT(*) FROM Member WHERE Email = @Email";
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

                    // Checksif the email already exists
                    bool emailExists = await EmailExistsAsync(member.Email);
                    if (emailExists)
                    {
                        return false;
                    }

                    using (SqlCommand command = new SqlCommand(insertSQL, connection))
                    {

                        command.Parameters.AddWithValue("@Name", member.Name);
                        command.Parameters.AddWithValue("@Email", member.Email);
                        command.Parameters.AddWithValue("@Password", member.Password);

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

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(CheckForEmailSQL, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        int count = (int)await command.ExecuteScalarAsync();
                        return count > 0;
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


        public async Task<bool> DeleteMemberAsync(int memberId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(deleteSQL, connection))
                    {

                        command.Parameters.AddWithValue("@MemberID", memberId);


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


        public async Task<int> FindHighestMemberIdAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(HighestID, connection))
                    {

                        object result = await command.ExecuteScalarAsync();


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

            }
            catch (Exception ex)
            {
                Console.WriteLine("General error: " + ex.Message);

            }

            return 0; 
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

            }
            catch (Exception ex)
            {
                Console.WriteLine("General error: " + ex.Message);

            }

            return null; 
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

            }
            catch (Exception ex)
            {
                Console.WriteLine("General error: " + ex.Message);

            }

            return null;
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

        public List<Member> GetAllMembers()
        {
            List<Member> members = new List<Member>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    try
                    {
                        command.Connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Member member = new Member()
                            {
                                MemberID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                            };
                            members.Add(member);
                        }

                        reader.Close();
                    }
                    catch (SqlException sqlex)
                    {
                        Console.WriteLine("Database error");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("General error");
                    }
                }
            }

            return members;
        }


        public Member GetLoggedMember(string email)
        {
            if (email != null)
            {
                return GetAllMembers().Find(u => u.Email == email);
            }
            else
                return null;
        }

        public Member VerifyMember(string email, string passWord)
        {
            foreach (var member in GetAllMembers())
            {
                if (email.Equals(member.Email) && passWord.Equals(member.Password))
                {
                    return member;
                }

            }
            return null;

        }

        public Member GetLoggedMemberID(int MemberID)
        {
            if (MemberID != null)
            {
                return GetAllMembers().Find(u => u.MemberID == MemberID);
            }
            else
                return null;


        }

    }
}
