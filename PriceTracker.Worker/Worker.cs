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
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(30));

            do
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var database = scope.ServiceProvider.GetRequiredService<Database>();

                var factory = scope.ServiceProvider.GetRequiredService<PriceTrackerFactory>();

                var comparisonService = scope.ServiceProvider.GetRequiredService<PriceComparisonService>();

                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                _logger.LogInformation("Iniciando nova rodada de monitoramento - Data/Hora: {dateTime}", DateTime.Now);

                var products = await database.GetProductsToTrack();                

                foreach (var product in products)
                {
                    try
                    {
                        var strategy = factory.GetStrategy(product.Platform);

                        var trackingResult = await strategy.ExtractPriceAsync(product.Url);

                        var priceHistory = PriceHistory.Create(product.Id, product.Description, product.Platform, trackingResult.Price);

                        var oldHistory = await database.GetLastPriceHistoryAsync(product.Id);

                        if (oldHistory is null)//se nao houver historico eu notifico o primeiro preço
                        {
                            await notificationService.NotifyAsync(PriceAlert.Create(product.Description, product.Platform, trackingResult.Price));
                        }
                        else
                        {
                            await comparisonService.ComparePricesAsync(priceHistory, oldHistory);
                        }

                        await database.InsertPriceHistoryAsync(priceHistory);

                        _logger.LogInformation("Produto consultado: {productDescription}", product.Description);

                        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar o produto {productDescription} - Data/Hora: {dateTime}", product.Description, DateTime.UtcNow);
                        continue;
                    }
                }
            } while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
