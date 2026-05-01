using Microsoft.Data.Sqlite;
using PriceTracker.Models;
using System.Globalization;

namespace PriceTracker.Infrastructure
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()//esse metodo cuida da estrutura do banco, então qualquer tabela nova deve ser criada aqui
        {
            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = """                
                 CREATE TABLE IF NOT EXISTS PriceHistory                 
                 (Id INTEGER PRIMARY KEY AUTOINCREMENT,
                 ProductDescription  TEXT NOT NULL,
                 Platform            TEXT NOT NULL,
                 Price               REAL NOT NULL,
                 RecordedAt          TEXT NOT NULL)                
                

                 CREATE TABLE IF NOT EXISTS Product (
                 Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                 Description TEXT NOT NULL,
                 Platform    TEXT NOT NULL,
                 Url         TEXT NOT NULL
                     );
                 """;

            command.ExecuteNonQuery();
        }

        public void InsertPriceHistory(PriceHistory priceHistory)
        {
            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = """                
                    INSERT INTO PriceHistory (ProductDescription, Platform, Price, RecordedAt)
                    VALUES (@ProductDescription, @Platform, @Price, @RecordedAt)
                   """;
            command.Parameters.AddWithValue("@ProductDescription", priceHistory.ProductDescription);
            command.Parameters.AddWithValue("@Platform", priceHistory.Platform.ToString());
            command.Parameters.AddWithValue("@Price", priceHistory.Price);
            command.Parameters.AddWithValue("@RecordedAt", priceHistory.RecordedAt.ToString("o"));

            command.ExecuteNonQuery();
        }

        public PriceHistory? GetPriceHistory()
        {
            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = """                
                    SELECT Id, ProductDescription, Platform, Url, Price, RecordedAt
                    FROM PriceHistory
                    ORDER BY RecordedAt DESC
                    LIMIT 1
                   """;
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var historico = PriceHistory.Reconstitute(reader.GetInt32(0), reader.GetString(1), Enum.Parse<Platform>(reader.GetString(2)), reader.GetDecimal(4), DateTime.Parse(reader.GetString(5), null, DateTimeStyles.RoundtripKind));

                return historico;
            }

            return null;
        }

        public List<Product> GetProductsToTrack()
        {
            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = """                
                    SELECT Id, Description, Platform, Url
                    FROM Product
                   """;
            using var reader = command.ExecuteReader();

            var products = new List<Product>();

            while (reader.Read())
            {
                var product = new Product
                {
                    Id = reader.GetInt32(0),
                    Description = reader.GetString(1),
                    Platform = Enum.Parse<Platform>(reader.GetString(2)),
                    Url = reader.GetString(3),
                };
                products.Add(product);
            }
            return products; 
        }
    }
}
