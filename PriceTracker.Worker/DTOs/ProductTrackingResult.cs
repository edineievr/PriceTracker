using PriceTracker.Worker.Enums;

namespace PriceTracker.Worker.DTOs
{
    public class ProductTrackingResult
    {
        public string ProductDescription { get; set; }
        public Platform Platform { get; set; } 
        public decimal SpotPrice { get; set; }
        public decimal? CreditCardPrice { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int? CreditCardInstallment { get; set; }


        public ProductTrackingResult()
        {
            ProductDescription = string.Empty;
        }
    }
}