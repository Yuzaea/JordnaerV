using Jordnaer.Interfaces;
using Jordnaer.Models;
using System.Data.SqlClient;

namespace Jordnaer.Services
{
    public class ItemService : Connection, IItemService
    {
        private String queryString = "SELECT * FROM Item";
        private String SelectSQL = "SELECT * FROM Item WHERE Item_ID = @Item_ID";
        private String insertSQL = "INSERT INTO Item (Item_Name, Item_Img, Item_Price, Item_Description, Item_Type) VALUES (@Item_Name, @Item_Img, @Item_Price, @Item_Description, @Item_Type)";
        private String DeleteSQL = "DELETE FROM Item WHERE Item_ID = @Item_ID";
        private string UpdateSQL = "UPDATE Item SET Item_Name = @Item_Name, Item_Img = @Item_Img, Item_Price = @Item_Price, Item_Description = @Item_Description, Item_Type = @Item_Type WHERE Item_ID = @Item_ID";
        private String ItemByNameSQl = "SELECT * FROM Item WHERE Item_Name LIKE @Name";
        private string findHighestItemIdSQL = "SELECT MAX(Item_ID) FROM Items";


        public ItemService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> CreateItemAsync(Item item)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(insertSQL, connection))
                {
                    command.Parameters.AddWithValue("@Item_Name", item.ItemName);
                    command.Parameters.AddWithValue("@Item_Img", item.ItemImg);
                    command.Parameters.AddWithValue("@Item_Price", item.ItemPrice);
                    command.Parameters.AddWithValue("@Item_Description", item.ItemDescription);
                    command.Parameters.AddWithValue("@Item_Type", item.ItemType);

                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0;
                    }
                    catch (SqlException sqlex)
                    {
                        Console.WriteLine("Database error: " + sqlex.Message);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("General error: " + ex.Message);

                    }
                }
            }

            return false; 
        }


        public async Task<Item> DeleteItemAsync(int itemID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand deleteCommand = new SqlCommand(DeleteSQL, connection))
                {
                    deleteCommand.Parameters.AddWithValue("@Item_ID", itemID);

                    try
                    {
                        deleteCommand.Connection.Open();

                        int noOfRows = await deleteCommand.ExecuteNonQueryAsync();
                        if (noOfRows == 1)
                        {

                            return new Item { ItemID = itemID };
                        }
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

            return null; 
        }


        public async Task<List<Item>> GetAllItemsAsync()
        {
            List<Item> items = new List<Item>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    try
                    {
                        command.Connection.Open();
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            Item item = new Item()
                            {
                                ItemName = reader.GetString(0),
                                ItemID = reader.GetInt32(1),
                                ItemPrice = (float)reader.GetDouble(2),
                                ItemDescription = reader.GetString(3),
                                ItemType = reader.GetString(4),
                                ItemImg = reader.GetString(5),

                            };
                            items.Add(item);
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

            return items;
        }


        public async Task<Item> GetItemFromIdAsync(int itemId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(SelectSQL, connection))
                {
                    command.Parameters.AddWithValue("@Item_ID", itemId);

                    try
                    {
                        await connection.OpenAsync();
                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();

                            Item item = new Item
                            {
                                ItemID = reader.GetInt32(reader.GetOrdinal("Item_ID")),
                                ItemName = reader.GetString(reader.GetOrdinal("Item_Name")),
                                ItemImg = reader.GetString(reader.GetOrdinal("Item_Img")),
                                ItemPrice = (float)reader.GetDouble(reader.GetOrdinal("Item_Price")),
                                ItemDescription = reader.GetString(reader.GetOrdinal("Item_Description")),
                                ItemType = reader.GetString(reader.GetOrdinal("Item_Type"))
                            };


                            return item;
                        }

                        return null;
                    }
                    catch (SqlException sqlex)
                    {
                        Console.WriteLine("Database error: " + sqlex.Message);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("General error: " + ex.Message);
                        throw;
                    }
                }
            }
        }

        public async Task<int> FindHighestItemIdAsync()
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(findHighestItemIdSQL, connection))
                {
                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();

                        if (result != null && int.TryParse(result.ToString(), out int highestItemId))
                        {
                            return highestItemId;
                        }
                    }
                    catch (SqlException sqlex)
                    {
                        Console.WriteLine("Database error: " + sqlex.Message);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("General error: " + ex.Message);
                        throw;
                    }
                }
            }

            return 0; 
        }

        public async Task<bool> UpdateItemAsync(Item item, int itemId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(UpdateSQL, connection))
                {
                    item.ItemID = itemId;
                    command.Parameters.AddWithValue("@Item_Name", item.ItemName);
                    command.Parameters.AddWithValue("@Item_Img", item.ItemImg);
                    command.Parameters.AddWithValue("@Item_Price", item.ItemPrice);
                    command.Parameters.AddWithValue("@Item_Description", item.ItemDescription);
                    command.Parameters.AddWithValue("@Item_Type", item.ItemType);
                    command.Parameters.AddWithValue("@Item_ID", item.ItemID);

                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0;
                    }
                    catch (SqlException sqlex)
                    {
                        Console.WriteLine("Database error: " + sqlex.Message);

                        throw; //Shit failed ):
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("General error: " + ex.Message);

                        throw; //Shit failed ):
                    }
                }
            }

        }

    }
}
