using PriceTracker.Intefaces;
using PriceTracker.Strategies;

namespace PriceTracker
{
    public class PriceTrackerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public PriceTrackerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPriceScraper CreatePriceTracker(string platform)
        {
            return platform switch
            {
                "Mercado Livre" => _serviceProvider.GetRequiredService<MeliStrategy>(),
                _ => throw new NotSupportedException($"Platform '{platform}' is not supported.")
            };
        }
    }
}
