namespace PriceTracker.DTOs.ProductRequest
{
    public class PriceAlertDto
    {
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public string Platform { get; set; }


        public PriceAlertDto()
        {
            ProductDescription = string.Empty;
            Platform = string.Empty;
        }
    }
}