namespace PriceTracker.Models
{
    public class PriceAlert
    {
        public string ProductDescription { get; set; }
        public Platform Platform { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}
