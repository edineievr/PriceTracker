using PriceTracker.Worker.Enums;

namespace PriceTracker.Worker.Models
{
    public class PriceAlert
    {
        public string ProductDescription { get; private set; }
        public Platform Platform { get; private set; }
        public decimal? OriginalPrice { get; private set; }
        public decimal SpotPrice { get; private set; }
        public decimal? CreditCardPrice { get; private set; }
        public int? CreditCardInstallment { get; private set; }

        public static PriceAlert Create(Platform platform, string productDescription, decimal spotPrice, decimal? creditCardPrice, int? creditCardInstallment, decimal? originalPrice)
        {
            return new PriceAlert
            {
                ProductDescription = productDescription,
                Platform = platform,
                SpotPrice = spotPrice,
                CreditCardPrice = creditCardPrice,
                CreditCardInstallment = creditCardInstallment,
                OriginalPrice = originalPrice
            };
        }
    }
}
