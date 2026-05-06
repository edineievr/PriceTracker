using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Models;

namespace PriceTracker.Worker.Services
{
    public class PriceComparisonService
    {
        private readonly INotificationService _notificationService;

        public PriceComparisonService(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task ComparePricesAsync(PriceHistory newHistory, PriceHistory? oldHistory)
        {
            if (oldHistory != null && oldHistory.Price > newHistory.Price)
            {
                await _notificationService.NotifyAsync(new PriceAlert
                {
                    ProductDescription = newHistory.ProductDescription,
                    Platform = newHistory.Platform,
                    CurrentPrice = newHistory.Price,
                });
            }
        }
    }
}
