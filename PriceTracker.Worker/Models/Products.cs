using PriceTracker.Worker.Enums;

namespace PriceTracker.Worker.Models
{
    public class Product
    {
        public int Id { get; protected set; }
        public string Description { get; private set; }
        public Platform Platform { get; private set; }
        public string Url { get; private set; }
        public bool IsActive { get; private set; }

        public static Product Create(Platform platform, string description,  string url)
        {
            return new Product
            {
                Description = description,
                Platform = platform,
                Url = url,
                IsActive = true
            };
        }

        public static Product Reconstitute(int id, Platform platform, string description,  string url, bool isActive)
        {
            return new Product
            {
                Id = id,
                Description = description,
                Platform = platform,
                Url = url,
                IsActive = isActive
            };
        }
    }
}
