using PriceTracker.Worker.DTOs;

namespace PriceTracker.Worker.Intefaces
{
    public interface IPriceScraper
    {
        Task<ProductTrackingResult> ExtractPrice(string url);
    }
}
