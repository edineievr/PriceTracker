using Microsoft.Extensions.Logging;
using PriceTracker.Worker.Strategies;
using PriceTracker.Worker.Enums;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTracker.Test.Strategies
{
    [TestFixture]
    public class KabumStrategyTest
    {
        private KabumStrategy _kabumStrategy;

        [SetUp]
        public void Setup()
        {
            var logger = new Logger<KabumStrategy>(new LoggerFactory());
            _kabumStrategy = new KabumStrategy(logger);
        }

        [Test]
        public async Task ExtractPrice_ShouldReturnValidResult()
        {
            // Arrange
            string url = "https://www.kabum.com.br/produto/426262/processador-amd-ryzen-7-7800x3d-5-0ghz-max-turbo-cache-104mb-am5-8-nucleos-video-integrado-100-100000910wof";
            
            // Act
            var result = await _kabumStrategy.ExtractPriceAsync(url);
            
            // Assert
            result.ShouldNotBeNull();
            result.ProductDescription.ShouldBe("Processador AMD Ryzen 7 7800X3D, 5.0GHz Max Turbo, Cache 104MB, AM5, 8 Núcleos, Vídeo Integrado - 100-100000910WOF");
            result.Price.ShouldBeGreaterThan(0);
            result.Platform.ShouldBe(Platform.Kabum);
        }
    }
}
