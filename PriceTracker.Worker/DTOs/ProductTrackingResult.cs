using PriceTracker.Worker.Enums;

namespace PriceTracker.Worker.DTOs
{
    public class ProductTrackingResult
    {
        public string ProductDescription { get; set; }
        public Platform Platform { get; set; } 
        public decimal Price { get; set; }

        public ProductTrackingResult()
        {
            ProductDescription = string.Empty;
        }
    }
}