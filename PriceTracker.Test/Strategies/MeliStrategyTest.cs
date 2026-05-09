using Microsoft.Extensions.Logging;
using PriceTracker.Worker.Strategies;
using PriceTracker.Worker.Enums;
using Shouldly;
using System;

namespace PriceTracker.Test.Strategies
{
    [TestFixture]
    public class MeliStrategyTest
    {
        private MeliStrategy _meliStrategy;

        [SetUp]
        public void Setup()
        {
            var logger = new Logger<MeliStrategy>(new LoggerFactory());
            _meliStrategy = new MeliStrategy(logger);
        }

        [Test]
        public async Task ExtractPrice_ShouldReturnValidResult()
        {
            // Arrange
            string url = "https://www.mercadolivre.com.br/suplemento-em-po-profit-laboratorios-anabolic-mass-28500-proteinas-sabor-chocolate-em-sach-de-3kg/p/MLB12406685?product_trigger_id=MLB27946823&pdp_filters=item_id%3AMLB3876921465&applied_product_filters=MLB27946823&picker=true&quantity=1";

            // Act
            var result = await _meliStrategy.ExtractPriceAsync(url);

            // Assert
            result.ShouldNotBeNull();
            result.ProductDescription.ShouldBe("Suplemento em pó ProFit Laboratórios Anabolic Mass 28500 proteínas sabor chocolate em sachê de 3kg");
            result.Price.ShouldBeGreaterThan(0);
            result.Platform.ShouldBe(Platform.Meli);
        }
    }
}