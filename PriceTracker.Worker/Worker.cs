using PriceTracker.Worker.Factories;
using PriceTracker.Worker.Infrastructure;
using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Models;
using PriceTracker.Worker.Services;

namespace Pricetracker.Worker
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<Worker> _logger;

        public Worker(IServiceScopeFactory serviceScopeFactory, ILogger<Worker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker iniciado - Data/Hora: {dateTime}", DateTime.Now.ToString("dd/MM/yyyy - HH:mm"));

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(30));

            do
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var database = scope.ServiceProvider.GetRequiredService<Database>();

                var factory = scope.ServiceProvider.GetRequiredService<PriceTrackerFactory>();

                var comparisonService = scope.ServiceProvider.GetRequiredService<PriceComparisonService>();

                var products = await database.GetProductsToTrack(stoppingToken);

                foreach (var product in products)
                {
                    try
                    {
                        var strategy = factory.GetStrategy(product.Platform);

                        var trackingResult = await strategy.ExtractPriceAsync(product.Url, stoppingToken);

                        var priceHistory = PriceHistory.Create(product.Id, product.Platform, product.Description, trackingResult.SpotPrice, trackingResult.CreditCardPrice, trackingResult.CreditCardInstallment, trackingResult.OriginalPrice);

                        var oldHistory = await database.GetLastPriceHistoryAsync(product.Id, stoppingToken);//trazer pra memoria o ultimo registro de historico de cada produto na lista e evitar bater no banco toda vez pra buscar o ultimo registro (N + 1)

                        await comparisonService.ComparePricesAsync(priceHistory, oldHistory, stoppingToken);

                        await database.InsertPriceHistoryAsync(priceHistory, stoppingToken);

                        _logger.LogInformation("Produto consultado: {productDescription} - Plataforma: {platform}", product.Description, product.Platform);

                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Produto {productDescription} pulado por erro - {dateTime}", product.Description, DateTime.Now.ToString("dd/MM/yyyy - HH:mm"));
                        continue;
                    }
                }
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
