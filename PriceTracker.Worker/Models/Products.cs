using PriceTracker.Worker.Enums;

namespace PriceTracker.Worker.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Platform Platform { get; set; }
        public string Url { get; set; }
    }
}
