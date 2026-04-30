using PriceTracker.DTOs;

namespace PriceTracker.Intefaces
{
    public interface IPriceScraper
    {
        Task<ProductTrackingResult> ExtractPrice(string url);
    }
}
