using PriceTracker.Worker.Enums;

namespace PriceTracker.Worker.Models
{
    public class PriceAlert
    {
        public string ProductDescription { get; private set; }
        public Platform Platform { get; private set; }
        public decimal CurrentPrice { get; private set; }

        public static PriceAlert Create(string productDescription, Platform platform, decimal currentPrice)
        {
            return new PriceAlert
            {
                ProductDescription = productDescription,
                Platform = platform,
                CurrentPrice = currentPrice
            };
        }
    }
}
