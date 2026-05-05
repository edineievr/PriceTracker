using PriceTracker.Worker.Factories;
using PriceTracker.Worker.Infrastructure;
using PriceTracker.Worker.Models;
using PriceTracker.Worker.Services;

namespace PriceTracker.Worker.Jobs
{
    public class MonitoringJob : BackgroundService
    {
        private readonly Database _database;
        private readonly PriceTrackerFactory _priceTrackerFactory;
        private readonly PriceComparisonService _comparisonService;

        public MonitoringJob(Database database, PriceTrackerFactory priceTrackerFactory, PriceComparisonService comparisonService)
        {
            _database = database;
            _priceTrackerFactory = priceTrackerFactory;
            _comparisonService = comparisonService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(30));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                var products = _database.GetProductsToTrack();

                if (products.Count == 0)
                {
                    throw new Exception("Nenhum produto cadastrado para monitoramento");
                }

                foreach (var product in products)
                {
                    var priceScraper = _priceTrackerFactory.CreatePriceTracker(product.Platform);

                    var result = await priceScraper.ExtractPrice(product.Url) ?? throw new Exception("Falha ao extrair informações da url informada");

                    var oldHistory = _database.GetPriceHistory();

                    var newHistory = PriceHistory.Create(result.ProductDescription, result.Platform, result.Price);

                    _database.InsertPriceHistory(newHistory);

                    await _comparisonService.PriceComparison(newHistory, oldHistory);
                }
            }
        }
    }
}
