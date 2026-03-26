using PriceTracker.Intefaces;
using PriceTracker.Models;

namespace PriceTracker.Services
{
    public class PriceComparisonService
    {
        private readonly IPriceScraper _priceScraper;
        private readonly NotificationService _notificationService;

        public PriceComparisonService(IPriceScraper priceScraper, NotificationService notificationService)
        {
            _priceScraper = priceScraper;
            _notificationService = notificationService;
        }

        public async void PriceComparison(string url)
        {            
            var result = await _priceScraper.ExtractPrice(url) ?? throw new Exception("Falha ao extrair informações da url informada");

            var historico = new PriceHistory
            {
                ProductName = result.ProductDescription,
                Platform = result.Platform,
                Link = url,
                CurrentPrice = result.Price
            };

            if (historico.PriceChanged())
            {
                //_notificationService.SendNotification(historico);
            }
        }
    }
}
