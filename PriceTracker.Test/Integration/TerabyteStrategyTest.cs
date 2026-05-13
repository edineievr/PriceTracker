using Microsoft.Extensions.Logging;
using PriceTracker.Worker.Strategies;
using Shouldly;

namespace PriceTracker.Test.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class TerabyteStrategyTest
    {
        private TerabyteStrategy _terabyteStrategy;

        [SetUp]
        public void Setup()
        {
            var _logger = new LoggerFactory().CreateLogger<TerabyteStrategy>();
            _terabyteStrategy = new TerabyteStrategy(_logger);
        }

        [Test] 
        public async Task ExtractPrice_ShouldReturnValidResult()
        {
            string url = "https://www.terabyteshop.com.br/produto/30032/processador-amd-ryzen-9-9900x-44ghz-56ghz-turbo-12-cores-24-threads-am5-sem-cooler-100-100000662wof";

            var result = await _terabyteStrategy.ExtractPriceAsync(url);

            result.ShouldNotBeNull();
            result.ProductDescription.ShouldBe("Processador AMD Ryzen 9 9900X, 4.4GHz (5.6GHz Turbo), 12-Cores 24-Threads, AM5, Sem Cooler, 100-100000662WOF");
            result.CreditCardInstallment?.ShouldBeGreaterThan(0);
            result.OriginalPrice?.ShouldBeGreaterThan(0);
            result.SpotPrice.ShouldBeGreaterThan(0);
        }
    }
}
