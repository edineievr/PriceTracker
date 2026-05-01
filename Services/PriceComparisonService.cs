using PriceTracker.DTOs;
using PriceTracker.Infrastructure;
using PriceTracker.Intefaces;
using PriceTracker.Models;

namespace PriceTracker.Services
{
    public class PriceComparisonService
    {

        private readonly NotificationService _notificationService;
        private readonly PriceTrackerFactory _priceTrackerFactory;
        private readonly Database _database;

        public PriceComparisonService(NotificationService notificationService, PriceTrackerFactory priceTrackerFactory, Database database)
        {
            _notificationService = notificationService;
            _priceTrackerFactory = priceTrackerFactory;
            _database = database;
        }

        public async void PriceComparison(string url)
        {
            var products = _database.GetProductsToTrack();

            if (products.Count == 0)
            {
                throw new Exception("Nenhum produto cadastrado para monitoramento");
            }

            foreach (var product in products)
            {
                var priceScraper = _priceTrackerFactory.CreatePriceTracker(product.Platform);

                var result = await priceScraper.ExtractPrice(url) ?? throw new Exception("Falha ao extrair informações da url informada");

                var oldHistory = _database.GetPriceHistory();

                var newHistory = PriceHistory.Create(result.ProductDescription, result.Platform, result.Price);

                _database.InsertPriceHistory(newHistory);

                if (oldHistory != null && oldHistory.Price > newHistory.Price)
                {
                    await _notificationService.Notify(new PriceAlert
                    {
                        ProductDescription = newHistory.ProductDescription,
                        Platform = newHistory.Platform,
                        CurrentPrice = newHistory.Price,
                    });
                }
            }
        }
    }
}
