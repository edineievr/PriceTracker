using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Strategies;

namespace PriceTracker.Worker.Factories
{
    public class PriceTrackerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public PriceTrackerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPriceScraper GetStrategy(Platform platform)
        {
            return platform switch
            {
                Platform.Kabum => _serviceProvider.GetRequiredService<KabumStrategy>(),
                Platform.Pichau => _serviceProvider.GetRequiredService<PichauStrategy>(),
                Platform.Terabyte => _serviceProvider.GetRequiredService<TerabyteStrategy>(),
                _ => throw new NotSupportedException($"Plataforma '{platform}' não suportada.")
            };
        }
    }
}
