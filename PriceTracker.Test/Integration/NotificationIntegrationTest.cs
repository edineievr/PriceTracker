using Microsoft.Extensions.Logging;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Models;
using PriceTracker.Worker.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace PriceTracker.Test.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class NotificationIntegrationTest
    {
        private INotificationService _service;
        private ILogger<NotificationService> _logger;

        [SetUp]
        public void Setup()
        {
            var envPath = Path.Combine(
                Directory.GetCurrentDirectory(),"..", "..", "..", "..", "PriceTracker.Worker", ".env"
            );

            DotNetEnv.Env.Load(envPath);
            Console.WriteLine(Environment.GetEnvironmentVariable("TELEGRAM_API_KEY"));
            _logger = new Logger<NotificationService>(new LoggerFactory());
            _service = new NotificationService(_logger);
        }

        [Test]
        public async Task Notify_Should_Send_Notification()
        {
            // Arrange
            var priceAlert = PriceAlert.Create(Platform.Kabum, "Teste de Produto",  99.99m, creditCardPrice: 120.00m, creditCardInstallment: 10, originalPrice: 150.00m);
            // Act
            await _service.NotifyAsync(priceAlert, CancellationToken.None);

            Assert.Pass("Notification sent successfully.");
        }
    }
}
