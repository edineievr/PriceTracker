using PriceTracker.DTOs.ProductRequest;

namespace PriceTracker.Intefaces
{
    public interface IPriceScraper
    {
        Task<PriceAlertDto> ExtractPrice(string url);
    }
}
