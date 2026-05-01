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

        public IPriceScraper CreatePriceTracker(Platform platform)
        {
            return platform switch
            {
                Platform.Meli => _serviceProvider.GetRequiredService<MeliStrategy>(),
                //Platform.Kabum => _serviceProvider.GetRequiredService<KabumStrategy>(),
                _ => throw new NotSupportedException($"Platform '{platform}' is not supported.")
            };
        }
    }
}
