namespace PriceTracker.Models
{
    public class PriceHistory
    {
        public int Id { get; protected set; }
        public string ProductDescription { get; private set; }
        public Platform Platform { get; private set; }
        public DateTime RecordedAt { get; private set; }
        public decimal Price { get; private set; }

        private PriceHistory() { }

        public static PriceHistory Create(string description, Platform platform, decimal price)
        {
            return new PriceHistory
            {
                ProductDescription = description,
                Platform = platform,
                Price = price,
                RecordedAt = DateTime.UtcNow
            };
        }

        public static PriceHistory Reconstitute(int id, string description, Platform platform, decimal price, DateTime recordedAt)
        {
            return new PriceHistory
            {
                Id = id,
                ProductDescription = description,
                Platform = platform,
                Price = price,
                RecordedAt = recordedAt
            };
        }
    }
}
