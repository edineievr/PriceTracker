using PriceTracker.DTOs;

namespace PriceTracker.Intefaces
{
    public interface IPriceScraper
    {
        Task<PriceAlertDto> ExtractPrice(string url);
    }
}
