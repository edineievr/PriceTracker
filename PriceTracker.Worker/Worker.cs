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

        public Worker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var database = scope.ServiceProvider.GetRequiredService<Database>();

                var factory = scope.ServiceProvider.GetRequiredService<PriceTrackerFactory>();

                var comparisonService = scope.ServiceProvider.GetRequiredService<PriceComparisonService>();

                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var products = await database.GetProductsToTrack();

                Console.WriteLine("Worker iniciado");

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

                        Console.WriteLine($"Produto consultado");
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao processar o produto {product.Description}: {ex.Message} - Data/Hora: {DateTime.Now}");
                        continue;
                    }
                }
            }
        }
    }
}
