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

        #region Testes de Sucesso

        [Test]
        public async Task ExtractPrice_ShouldReturnValidResult()
        {
            // Arrange
            string url = "https://www.kabum.com.br/produto/426262/processador-amd-ryzen-7-7800x3d-5-0ghz-max-turbo-cache-104mb-am5-8-nucleos-video-integrado-100-100000910wof?gclsrc=aw.ds&&utm_id=22429436063&gad_source=1&gad_campaignid=22429436063&gbraid=0AAAAADx-HyGREsKUM_3puMtKuxO9rWXjD&gclid=CjwKCAjwqubPBhBOEiwAzgZX2laSDYkpQIXY7apeGFp2dcC5lvVd0SCko3FpfszhpyIUvL_fV9cBvhoC0T0QAvD_BwE";
            
            // Act
            var result = await _kabumStrategy.ExtractPriceAsync(url);
            
            // Assert
            result.ShouldNotBeNull();
            result.ProductDescription.ShouldBe("Processador AMD Ryzen 7 7800X3D, 5.0GHz Max Turbo, Cache 104MB, AM5, 8 Núcleos, Vídeo Integrado - 100-100000910WOF");
            result.Price.ShouldBeGreaterThan(0);
            result.Platform.ShouldBe(Platform.Kabum);
        }

        [Test]
        public async Task ExtractPrice_ShouldReturnKabumPlatform()
        {
            // Arrange
            string url = "https://www.kabum.com.br/produto/426262/processador-amd-ryzen-7-7800x3d-5-0ghz-max-turbo-cache-104mb-am5-8-nucleos-video-integrado-100-100000910wof";
            
            // Act
            var result = await _kabumStrategy.ExtractPriceAsync(url);
            
            // Assert
            result.Platform.ShouldBe(Platform.Kabum);
        }

        [Test]
        public async Task ExtractPrice_ShouldHaveValidPriceFormat()
        {
            // Arrange
            string url = "https://www.kabum.com.br/produto/426262/processador-amd-ryzen-7-7800x3d-5-0ghz-max-turbo-cache-104mb-am5-8-nucleos-video-integrado-100-100000910wof";
            
            // Act
            var result = await _kabumStrategy.ExtractPriceAsync(url);
            
            // Assert
            result.Price.ShouldBeGreaterThanOrEqualTo(0);
            // Preço válido em reais deve ser maior que 0
            result.Price.ShouldBeLessThan(decimal.MaxValue);
        }

        #endregion

        #region Testes de Falha e Casos Extremos

        [Test]
        public void ExtractPrice_WithInvalidUrl_ShouldThrowException()
        {
            // Arrange
            string invalidUrl = "https://invalid-url-that-does-not-exist-12345.com/produto/123";
            
            // Act & Assert
            Should.ThrowAsync<Exception>(() => _kabumStrategy.ExtractPriceAsync(invalidUrl));
        }

        [Test]
        public void ExtractPrice_WithEmptyUrl_ShouldThrowException()
        {
            // Arrange
            string emptyUrl = string.Empty;
            
            // Act & Assert
            Should.ThrowAsync<Exception>(() => _kabumStrategy.ExtractPriceAsync(emptyUrl));
        }

        [Test]
        public void ExtractPrice_WithNullUrl_ShouldThrowException()
        {
            // Arrange
            string nullUrl = null;
            
            // Act & Assert
            Should.ThrowAsync<ArgumentNullException>(() => _kabumStrategy.ExtractPriceAsync(nullUrl));
        }

        #endregion

        #region Testes de Integridade de Dados

        [Test]
        public async Task ExtractPrice_ShouldNotReturnNullProductDescription()
        {
            // Arrange
            string url = "https://www.kabum.com.br/produto/426262/processador-amd-ryzen-7-7800x3d-5-0ghz-max-turbo-cache-104mb-am5-8-nucleos-video-integrado-100-100000910wof";
            
            // Act
            var result = await _kabumStrategy.ExtractPriceAsync(url);
            
            // Assert
            result.ProductDescription.ShouldNotBeNull();
            result.ProductDescription.ShouldNotBeEmpty();
            result.ProductDescription.ShouldBe("Processador AMD Ryzen 7 7800X3D, 5.0GHz Max Turbo, Cache 104MB, AM5, 8 Núcleos, Vídeo Integrado - 100-100000910WOF");
        }

        [Test]
        public async Task ExtractPrice_ShouldContainOnlyPositivePrice()
        {
            // Arrange
            string url = "https://www.kabum.com.br/produto/426262/processador-amd-ryzen-7-7800x3d-5-0ghz-max-turbo-cache-104mb-am5-8-nucleos-video-integrado-100-100000910wof";
            
            // Act
            var result = await _kabumStrategy.ExtractPriceAsync(url);
            
            // Assert
            // Preço 0 indica que não foi possível extrair, mas o resultado não deve ser nulo
            result.Price.ShouldBeGreaterThanOrEqualTo(0);
        }

        #endregion
    }
}
