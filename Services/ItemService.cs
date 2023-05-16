﻿using Jordnaer.Interfaces;
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
        private String UpdateSQL = "UPDATE Item SET Item_Name = @Name, Item_Img = @Img, Item_Price = @Price, Item_Description = @Description, Item_Type = @Type WHERE Item_ID = @ID";
        private String ItemByNameSQl = "SELECT * FROM Item WHERE Item_Name LIKE @Name";
        private string SelectByNameAndTypeSQL = "SELECT * FROM Item WHERE Item_Name = @Item_Name AND Item_Type = @Item_Type";


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

                        return rowsAffected > 0; // Return true if rows were affected (insert successful)
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
                }
            }

            return false; // Insert failed
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
                            // Deletion successful, return a dummy Item object with the deleted item's ID
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

            return null; // Deletion failed
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
                                ItemID = reader.GetInt32(0),
                                ItemName = reader.GetString(1),
                                ItemImg = reader.GetString(2),
                                ItemPrice = (float)reader.GetDouble(3),
                                ItemDescription = reader.GetString(4),
                                ItemType = reader.GetString(5)
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
                                ItemPrice = reader.GetFloat(reader.GetOrdinal("Item_Price")),
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

        public async Task<Item> GetItemFromNameAndTypeAsync(string itemName, string itemType)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(SelectByNameAndTypeSQL, connection))
                {
                    command.Parameters.AddWithValue("@Item_Name", itemName);
                    command.Parameters.AddWithValue("@Item_Type", itemType);

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
                                ItemName = itemName,
                                ItemImg = reader.GetString(reader.GetOrdinal("Item_Img")),
                                ItemPrice = reader.GetFloat(reader.GetOrdinal("Item_Price")),
                                ItemDescription = reader.GetString(reader.GetOrdinal("Item_Description")),
                                ItemType = itemType
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




        public Task<List<Item>> GetItemsByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(Item item, int itemId)
        {
            throw new NotImplementedException();
        }
    }
}
