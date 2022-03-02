using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(IConfiguration config) : base(config) { }
        // Function for pulling all of the Categories from the database
        public List<Category> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Category ORDER BY Name";
                    var reader = cmd.ExecuteReader();

                    var categories = new List<Category>();

                    while (reader.Read())
                    {
                        categories.Add(new Category()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        });
                    }
                    reader.Close();
                    return categories;
                }
            }
        }

        public void AddCategory(Category category)
        {
            using (var conn = Connection)
            {
                using var cmd = conn.CreateCommand();
                {
                    cmd.CommandText = @"
                        INSERT INTO Category([Name])
                        OUTPUT INSERTED.ID
                        VALUES(@Name)";
                    cmd.Parameters.AddWithValue("@Name", category.Name);
                    category.Id = (int)cmd.ExecuteScalar();
                }
            }
        }
    }
}
