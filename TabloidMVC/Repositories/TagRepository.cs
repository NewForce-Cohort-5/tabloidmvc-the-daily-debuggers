﻿using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public class TagRepository : BaseRepository, ITagRepository
    {
        public TagRepository(IConfiguration config) : base(config) { }

        public List<Tag> GetAllTags()
        {
            List<Tag> tags = new List<Tag>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT [Id], [Name] FROM Tag
                    ";

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Tag tag = new Tag()
                        {
                            Id = dr.GetInt32(dr.GetOrdinal("Id")),
                            Name = dr.GetString(dr.GetOrdinal("Name"))
                        };

                        tags.Add(tag);
                    }

                    dr.Close();

                    return tags;
                }
            }
        }
    }
}