using Microsoft.Extensions.Logging;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Strategies;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Net.WebRequestMethods;

namespace PriceTracker.Test.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class PichauStrategyTest
    {
        private PichauStrategy _pichauStrategy;

        [SetUp]
        public void Setup()
        {
            var logger = new Logger<PichauStrategy>(new LoggerFactory());
            _pichauStrategy = new PichauStrategy(logger);
        }

        [Test]
        public async Task ExtractPrice_ShouldReturnValidResult()
        {
            string url = "https://www.pichau.com.br/placa-mae-msi-pro-b650m-p-ddr5-socket-amd-am5-m-atx-chipset-amd-b650-pro-b650m-p";

            var result = await _pichauStrategy.ExtractPriceAsync(url);

            result.ShouldNotBeNull();
            result.ProductDescription.ShouldBe("Placa Mae MSI Pro B650M-P, DDR5, Socket AMD AM5, M-ATX, Chipset AMD B650, PRO-B650M-P");
            result.SpotPrice.ShouldBeGreaterThan(0);
            result.Platform.ShouldBe(Platform.Pichau);
            result.OriginalPrice?.ShouldBeGreaterThan(0);
            result.CreditCardPrice?.ShouldBeGreaterThan(0);
            result.CreditCardInstallment?.ShouldBeGreaterThan(0);
        }
    }
}
