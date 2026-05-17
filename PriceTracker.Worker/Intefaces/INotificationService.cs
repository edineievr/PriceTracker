using PriceTracker.Worker.Models;

namespace PriceTracker.Worker.Intefaces
{
    public interface INotificationService
    {
        Task NotifyAsync(PriceAlert alert, CancellationToken stoppingToken);
    }
}
