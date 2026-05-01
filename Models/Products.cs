namespace PriceTracker.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Platform Platform { get; set; }
        public string Url { get; set; }

    }
}
