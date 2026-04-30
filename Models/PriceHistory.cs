namespace PriceTracker.Models
{
    public class PriceHistory
    {
        public long Id { get; protected set; }
        public string ProductDescription { get; private set; }
        public string Platform { get; private set; }
        public string Url { get; private set; }
        public DateTime RecordedAt { get; private set; }
        public decimal Price { get; private set; }

        private PriceHistory() { }

        public static PriceHistory Create(string description, string platform, string url, decimal price)
        {
            return new PriceHistory
            {
                ProductDescription = description,
                Platform = platform,
                Url = url,
                Price = price,
                RecordedAt = DateTime.UtcNow
            };
        }
    }
}
