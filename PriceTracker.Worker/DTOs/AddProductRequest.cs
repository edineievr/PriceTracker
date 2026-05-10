using PriceTracker.Worker.Enums;

namespace PriceTracker.Worker.DTOs
{
    public class AddProductRequest
    {
        public string Description { get; set; }
        public Platform Platform { get; set; }
        public string Url { get; set; }
    }
}
