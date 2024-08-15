using Microsoft.Data.SqlClient;
using TabloidCLI.Models;
using TabloidCLI.Repositories;


namespace TabloidCLI.Repositories
{
    public class BlogRepository : DatabaseConnector, IRepository<Blog>
    {
        public BlogRepository(string connectionString) : base(connectionString) { }


        public List<Blog> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Blog";
                    List<Blog> blogs = [];

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Blog blog = new Blog()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Url = reader.GetString(reader.GetOrdinal("URL"))
                        };
                        blogs.Add(blog); 
                    }
                    reader.Close();

                    return blogs;
                }
            }
        }

        public Blog Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT * FROM Blog
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id); 

                    Blog blog = null;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("Id")))
                        {
                            blog = new Blog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Url = reader.GetString(reader.GetOrdinal("URL")),
                            };


                        }
                    }
                    reader.Close();
                    return blog; 

                }
            }
        }

        public Blog GetWithBlogTags(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT b.*, t.Name AS TagName, t.Id AS TagId
                                        FROM Blog b
                                        LEFT JOIN BlogTag bt ON b.Id = bt.BlogId
                                        LEFT JOIN Tag t ON bt.TagId = t.Id
                                        WHERE b.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id); 

                    Blog blog = null;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                           if (!reader.IsDBNull(reader.GetOrdinal("Id")))
                        {
                            blog = new Blog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Url = reader.GetString(reader.GetOrdinal("URL")),

                            };


                            if (!reader.IsDBNull(reader.GetOrdinal("TagId")))
                            {
                                blog.Tags.Add(new Tag()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("TagId")),
                                    Name = reader.GetString(reader.GetOrdinal("TagName"))
                                });
                            }
                        }
                    }
                    reader.Close();

                    return blog; 

                }
            }

        }

        public void Insert(Blog blog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Blog(Title, URL)
                                        VALUES (@title, @url)";
                    cmd.Parameters.AddWithValue("@title", blog.Title);
                    cmd.Parameters.AddWithValue("@url", blog.Url);

                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Blog successfully added.");
        }

        public void Update(Blog blog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Blog
                                        SET Title = @title, URL = @url
                                         WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", blog.Id); 
                    cmd.Parameters.AddWithValue("@title", blog.Title);
                    cmd.Parameters.AddWithValue("@url", blog.Url); 

                    cmd.ExecuteNonQuery();  
                                        
                }
            }
            Console.WriteLine("Blog successfully updated.");

        }

        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Blog WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }

            }
            Console.WriteLine("Blog successfully deleted.");
        }

        public void InsertTag(Blog blog, Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO BlogTag (BlogId, TagId)
                                        VALUES (@blogId, @tagId)";
                    cmd.Parameters.AddWithValue("@blogId", blog.Id);
                    cmd.Parameters.AddWithValue("@tagId", tag.Id);

                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Blog tag inserted successfully.");
        }

        public void DeleteTag(int blogId, int tagId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM BlogTag
                                        WHERE BlogId = @blogId AND 
                                              TagId = @tagId";
                    cmd.Parameters.AddWithValue("@blogId", blogId);
                    cmd.Parameters.AddWithValue("@tagId", tagId);

                    cmd.ExecuteNonQuery(); 
                }
            }
            Console.WriteLine("Blog tag removed.");
        }

    }
}
