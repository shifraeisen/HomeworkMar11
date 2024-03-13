using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkMar11.Data
{
    public class ImageRepository
    {
        private readonly string _connectionString;

        public ImageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Add(Image image)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"insert into Images (ImagePath, Password, ViewCount)
                                    values (@img, @pswd, @views)
                                    select SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@img", image.ImagePath);
            cmd.Parameters.AddWithValue("@pswd", image.Password);
            cmd.Parameters.AddWithValue("@views", 0);

            connection.Open();

            image.Id = (int)(decimal)cmd.ExecuteScalar();
        }
        public Image GetImageByID(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "select * from Images where Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();

            var reader = cmd.ExecuteReader();

            if(!reader.Read())
            {
                return null;
            }
            return new Image
            {
                Id = id,
                ImagePath = (string)reader["ImagePath"],
                Password = (string)reader["Password"],
                Views = (int)reader["ViewCount"]
            };
        }
        public void AddView(int imageID)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"update Images
                                set ViewCount = ViewCount + 1
                                where ID = @id";
            cmd.Parameters.AddWithValue("@id", imageID);

            connection.Open();

            cmd.ExecuteNonQuery();
        }
    }
}
