using Microsoft.Data.Sqlite;

namespace PriceTracker.Infrastructure
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Initialize()
        {
            using var connection = new SqliteConnection(_connectionString);

            connection.Open();

            var command = connection.CreateCommand();

            command.CommandText = """                
                 CREATE TABLE IF NOT EXISTS PriceHistory                 
                 (Id INTEGER PRIMARY KEY AUTOINCREMENT,
                 ProductDescription  TEXT NOT NULL,
                 Platform            TEXT NOT NULL,
                 Url                 TEXT NOT NULL,
                 Price               REAL NOT NULL,
                 RecordedAt          TEXT NOT NULL)                
                """;
            command.ExecuteNonQuery();
        }
    }
}
