using Microsoft.Data.Sqlite;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Models;
using System.Globalization;

namespace PriceTracker.Worker.Infrastructure
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InitializeAsync(CancellationToken stoppingToken)
        {
            using var connection = new SqliteConnection(_connectionString);

            await connection.OpenAsync(stoppingToken);

            var walCommand = connection.CreateCommand();
            walCommand.CommandText = "PRAGMA journal_mode=WAL;";
            await walCommand.ExecuteNonQueryAsync(stoppingToken);

            var command = connection.CreateCommand();
            command.CommandText = """
        CREATE TABLE IF NOT EXISTS Product (
            Id          INTEGER PRIMARY KEY AUTOINCREMENT,
            Description TEXT NOT NULL,
            Platform    TEXT NOT NULL,
            Url         TEXT NOT NULL,
            IsActive   INTEGER NOT NULL DEFAULT 1
        );

        CREATE TABLE IF NOT EXISTS PriceHistory (
            Id                 INTEGER PRIMARY KEY AUTOINCREMENT,
            ProductId          INTEGER NOT NULL,
            ProductDescription TEXT NOT NULL,
            Platform           TEXT NOT NULL,
            SpotPrice          REAL NOT NULL,            
            CreditCardPrice    REAL,
            CreditCardInstallment INTEGER,
            OriginalPrice      REAL,
            RecordedAt         TEXT NOT NULL,
            FOREIGN KEY (ProductId) REFERENCES Product(Id)
        );
        """;

            await command.ExecuteNonQueryAsync(stoppingToken);
        }

        public async Task InsertProductAsync(Product product, CancellationToken stoppingToken)
        {
            using var connection = new SqliteConnection(_connectionString);

            await connection.OpenAsync(stoppingToken);

            var command = connection.CreateCommand();

            command.CommandText = """                
                    INSERT INTO Product (Description, Platform, Url, IsActive)
                    VALUES (@Description, @Platform, @Url, @IsActive)
                   """;
            command.Parameters.AddWithValue("@Description", product.Description);
            command.Parameters.AddWithValue("@Platform", product.Platform.ToString());
            command.Parameters.AddWithValue("@Url", product.Url);
            command.Parameters.AddWithValue("@IsActive", product.IsActive);

            await command.ExecuteNonQueryAsync(stoppingToken);
        }

        public async Task InsertPriceHistoryAsync(PriceHistory priceHistory, CancellationToken stoppingToken)
        {
            using var connection = new SqliteConnection(_connectionString);

            await connection.OpenAsync(stoppingToken);
            var command = connection.CreateCommand();

            command.CommandText = """                
                    INSERT INTO PriceHistory (ProductId, ProductDescription, Platform, SpotPrice, RecordedAt, CreditCardPrice, CreditCardInstallment, OriginalPrice)
                    VALUES (@ProductId, @ProductDescription, @Platform, @SpotPrice, @RecordedAt, @CreditCardPrice, @CreditCardInstallment, @OriginalPrice)
                   """;
            command.Parameters.AddWithValue("@ProductId", priceHistory.ProductId);
            command.Parameters.AddWithValue("@ProductDescription", priceHistory.ProductDescription);
            command.Parameters.AddWithValue("@Platform", priceHistory.Platform.ToString());
            command.Parameters.AddWithValue("@SpotPrice", priceHistory.SpotPrice);
            command.Parameters.AddWithValue("@RecordedAt", priceHistory.RecordedAt.ToString("o"));
            command.Parameters.AddWithValue("@CreditCardPrice", priceHistory.CreditCardPrice ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CreditCardInstallment", priceHistory.CreditCardInstallment ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@OriginalPrice", priceHistory.OriginalPrice ?? (object)DBNull.Value);

            await command.ExecuteNonQueryAsync(stoppingToken);
        }

        public async Task<PriceHistory?> GetLastPriceHistoryAsync(int productId, CancellationToken stoppingToken)
        {
            using var connection = new SqliteConnection(_connectionString);

            await connection.OpenAsync(stoppingToken);
            var command = connection.CreateCommand();

            command.CommandText = """                
                    SELECT Id, ProductId, ProductDescription, Platform, SpotPrice, RecordedAt, CreditCardPrice, CreditCardInstallment, OriginalPrice
                    FROM PriceHistory
                    WHERE ProductId = @ProductId
                    ORDER BY RecordedAt DESC
                    LIMIT 1
                   """;
            command.Parameters.AddWithValue("@ProductId", productId);
            using var reader = await command.ExecuteReaderAsync(stoppingToken);

            if (reader.Read())
            {
                var historico = PriceHistory.Reconstitute(reader.GetInt32(0), 
                                                          reader.GetInt32(1),
                                                          Enum.Parse<Platform>(reader.GetString(3)),
                                                          reader.GetString(2),
                                                          reader.GetDecimal(4), 
                                                          reader.IsDBNull(6) ? null : reader.GetDecimal(6), 
                                                          reader.IsDBNull(7) ? null : reader.GetInt32(7), 
                                                          reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                                                          DateTime.Parse(reader.GetString(5), null, DateTimeStyles.RoundtripKind));

                return historico;
            }

            return null;
        }

        public async Task<List<Product>> GetProductsToTrack(CancellationToken stoppingToken)
        {
            using var connection = new SqliteConnection(_connectionString);

            await connection.OpenAsync(stoppingToken);
            var command = connection.CreateCommand();

            command.CommandText = """                
                    SELECT Id, Description, Platform, Url, IsActive
                    FROM Product
                    WHERE IsActive = 1
                   """;
            using var reader = await command.ExecuteReaderAsync(stoppingToken);

            var products = new List<Product>();

            while (reader.Read())
            {
                var product = Product.Reconstitute(reader.GetInt32(0), Enum.Parse<Platform>(reader.GetString(2)), reader.GetString(1),  reader.GetString(3), reader.GetBoolean(4));
                products.Add(product);
            }

            return products; 
        }
    }
}
