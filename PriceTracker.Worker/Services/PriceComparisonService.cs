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
            
            if (oldHistory is not null && oldHistory.SpotPrice > newHistory.SpotPrice)
            {
                await _notificationService.NotifyAsync(PriceAlert.Create(newHistory.Platform, newHistory.ProductDescription,  newHistory.SpotPrice, newHistory.CreditCardPrice, newHistory.CreditCardInstallment, newHistory.OriginalPrice));
            }
        }
    }
}
