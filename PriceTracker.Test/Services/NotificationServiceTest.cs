using PriceTracker.Test.Fakes;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Models;
using Shouldly;

namespace PriceTracker.Test.Services
{
    [TestFixture]
    public class NotificationServiceTest
    {
        private INotificationService _service;

        [SetUp]
        public void Setup()
        {
            _service = new FakeNotificationService();
        }

        [Test]
        public async Task When_Notify_Should_Set_WasCalled_And_LastAlert()
        {
            var priceAlert = PriceAlert.Create("Teste de Produto", Platform.Kabum, 99.99m, creditCardPrice: null, creditCardInstallment: null, originalPrice: null);

            await _service.NotifyAsync(priceAlert);

            var fake = (FakeNotificationService)_service;

            fake.WasCalled.ShouldBeTrue();
            fake.LastAlert?.SpotPrice.ShouldBe(99.99m);
        }


    }
}
