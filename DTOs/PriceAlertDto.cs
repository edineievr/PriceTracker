namespace PriceTracker.DTOs
{
    public class PriceAlertDto
    {
        public string ProductDescription { get; set; }
        public string CurrentPrice { get; set; }
        public string Platform { get; set; }
        public string OldPrice { get; set; }

        public PriceAlertDto()
        {
            ProductDescription = string.Empty;
            CurrentPrice = string.Empty;
            Platform = string.Empty;
            OldPrice = string.Empty;
        }
    }
}