using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Models;
using PriceTracker.Worker.Services;
using Shouldly;

namespace PriceTracker.Test.Services
{
    [TestFixture]
    public class PriceComparisonServiceTest
    {
        private PriceComparisonService _service;
        private Fakes.FakeNotificationService _fakeNotificationService;

        [SetUp]
        public void Setup()
        {
            _fakeNotificationService = new Fakes.FakeNotificationService();
            _service = new PriceComparisonService(_fakeNotificationService);
        }

        [Test]
        public async Task When_PriceDecreases_Should_Notify()
        {
            // Arrange
            var oldHistory = PriceHistory.Reconstitute(id: 1, productId: 1, description: "Teste de Produto", platform: Platform.Kabum, spotPrice: 100.00m, creditCardPrice: null, creditCardInstallment: null, originalPrice: 120.00m, recordedAt: DateTime.UtcNow.AddDays(-1));
            var newHistory = PriceHistory.Reconstitute(id: 2, productId: 1, description: "Teste de Produto", platform: Platform.Kabum, spotPrice: 90.00m, creditCardPrice: null, creditCardInstallment: null, originalPrice: 110.00m, recordedAt: DateTime.UtcNow);

            // Act & Assert
            Assert.DoesNotThrowAsync(() => _service.ComparePricesAsync(newHistory, oldHistory, CancellationToken.None));
            _fakeNotificationService.WasCalled.ShouldBeTrue();
            _fakeNotificationService.LastAlert?.SpotPrice.ShouldBe(90.00m);
        }
    }
}
