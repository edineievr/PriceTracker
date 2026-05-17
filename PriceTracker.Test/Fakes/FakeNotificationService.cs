using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Models;

namespace PriceTracker.Test.Fakes
{
    public class FakeNotificationService : INotificationService
    {
        public bool WasCalled { get; private set; }
        public PriceAlert? LastAlert { get; private set; }

        public Task NotifyAsync(PriceAlert alert, CancellationToken stoppingToken)
        {
            WasCalled = true;
            LastAlert = alert;
            return Task.CompletedTask;
        }
    }
}
