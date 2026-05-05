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
            var oldHistory = PriceHistory.Reconstitute(1, "Teste de Produto", Platform.Kabum, 100.00m, DateTime.UtcNow.AddDays(-1));
            var newHistory = PriceHistory.Reconstitute(2, "Teste de Produto", Platform.Kabum, 90.00m, DateTime.UtcNow);

            // Act & Assert
            Assert.DoesNotThrowAsync(() => _service.PriceComparison(newHistory, oldHistory));
            _fakeNotificationService.WasCalled.ShouldBeTrue();
            _fakeNotificationService.LastAlert?.CurrentPrice.ShouldBe(90.00m);
        }
    }
}
