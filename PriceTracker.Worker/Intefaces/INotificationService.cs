using PriceTracker.Worker.Models;

namespace PriceTracker.Worker.Intefaces
{
    public interface INotificationService
    {
        Task Notify(PriceAlert alert);
    }
}
