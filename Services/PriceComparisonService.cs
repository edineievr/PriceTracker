using PriceTracker.DTOs;
using PriceTracker.Intefaces;
using PriceTracker.Models;

namespace PriceTracker.Services
{
    public class PriceComparisonService
    {
        private readonly IPriceScraper _priceScraper;
        private readonly NotificationService _notificationService;
        private readonly PriceTrackerFactory _priceTrackerFactory;

        public PriceComparisonService(IPriceScraper priceScraper, NotificationService notificationService, PriceTrackerFactory priceTrackerFactory)
        {
            _priceScraper = priceScraper;
            _notificationService = notificationService;
            _priceTrackerFactory = priceTrackerFactory;
        }

        public async void PriceComparison(string url)
        {
            //buscar no banco os produtos a serem monitorados

            //o prduto conterá a respectiva plataforma

            //aqui eu resolvo qual estrategia de extração de preço a ser utilizada, de acordo com a plataforma do produto

            var result = await _priceScraper.ExtractPrice(url) ?? throw new Exception("Falha ao extrair informações da url informada");

            var historico = new PriceHistory
            {
                ProductName = result.ProductDescription,
                Platform = result.Platform,
                Link = url,
                CurrentPrice = result.CurrentPrice
            };


            if (!string.IsNullOrWhiteSpace(historico.OldPrice))
            {
                if (historico.PriceChanged())
                {
                    var priceAlert = new ProductTrackingResult
                    {
                        CurrentPrice = historico.CurrentPrice,
                        OldPrice = historico.OldPrice,
                        ProductDescription = historico.ProductName
                    };

                    await _notificationService.SendNotification(priceAlert);
                }
            }            

            //persistir o histórico de preços no banco de dados independente de ter ou não alteração de preço
        }
    }
}
