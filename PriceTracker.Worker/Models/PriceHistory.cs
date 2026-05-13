using PriceTracker.Worker.Enums;

namespace PriceTracker.Worker.Models
{
    public class PriceHistory
    {
        public int Id { get; protected set; }
        public int ProductId { get; private set; }
        public string ProductDescription { get; private set; }
        public Platform Platform { get; private set; }
        public decimal? OriginalPrice { get; private set; }
        public DateTime RecordedAt { get; private set; }
        public decimal SpotPrice { get; private set; }
        public decimal? CreditCardPrice { get; private set; }
        public int? CreditCardInstallment { get; private set; }

        private PriceHistory() { }

        public static PriceHistory Create(int productId, Platform platform, string description,  decimal spotPrice, decimal? creditCardPrice, int? creditCardInstallment, decimal? originalPrice)
        {
            return new PriceHistory
            {
                ProductId = productId,
                ProductDescription = description,
                Platform = platform,
                SpotPrice = spotPrice,
                CreditCardPrice = creditCardPrice,
                CreditCardInstallment = creditCardInstallment,
                OriginalPrice = originalPrice,
                RecordedAt = DateTime.UtcNow
            };
        }

        public static PriceHistory Reconstitute(int id, int productId, Platform platform, string description,  decimal spotPrice, decimal? creditCardPrice, int? creditCardInstallment, decimal? originalPrice, DateTime recordedAt)
        {
            return new PriceHistory
            {
                Id = id,
                ProductId = productId,
                ProductDescription = description,
                Platform = platform,
                SpotPrice = spotPrice,
                CreditCardPrice = creditCardPrice,
                CreditCardInstallment = creditCardInstallment,  
                OriginalPrice = originalPrice,
                RecordedAt = recordedAt
            };
        }
    }
}
