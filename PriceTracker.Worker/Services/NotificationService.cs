using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Models;
using System.Net.Http.Json;

namespace PriceTracker.Worker.Services
{
    public class NotificationService : INotificationService
    {
        private readonly string _apiKey;
        private readonly string _chatId;
        private readonly ILogger<NotificationService> _logger;
        private readonly HttpClient _httpClient = new();

        public NotificationService(ILogger<NotificationService> logger)
        {           
            _apiKey = Environment.GetEnvironmentVariable("TELEGRAM_API_KEY");
            _chatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID");
            _logger = logger;
        }
        public async Task NotifyAsync(PriceAlert priceAlert, CancellationToken stoppingToken)
        {
            _logger.LogInformation("Enviando notificação...{productDescription}", priceAlert.ProductDescription);

            var precoOriginal = priceAlert.OriginalPrice.HasValue ? $"\n💸 *De:* R$ {priceAlert.OriginalPrice:N2}" : string.Empty;

            var cartaoInfo = priceAlert.CreditCardPrice.HasValue && priceAlert.CreditCardInstallment.HasValue ? 
                             $"\n💳 *Cartão:* R$ {priceAlert.CreditCardPrice:N2}\n🔢 *Parcelas:* {priceAlert.CreditCardInstallment}x sem juros" : 
                             string.Empty;

            var payload = new
            {
                chat_id = _chatId,
                text = $"🔔 *Alerta de Preço*\n\n" +
                       $"📦 *Produto:* {priceAlert.ProductDescription}\n" +
                       $"🏪 *Plataforma:* {priceAlert.Platform}" +
                       precoOriginal +
                       $"\n💰 *À Vista:* R$ {priceAlert.SpotPrice:N2}" +
                       cartaoInfo,
                parse_mode = "Markdown"
            };

            var response = await _httpClient.PostAsJsonAsync($"https://api.telegram.org/bot{_apiKey}/sendMessage",payload, stoppingToken);

            _logger.LogInformation("Notificação enviada!");

            _logger.LogWarning("Resposta da API: {response}", await response.Content.ReadAsStringAsync(stoppingToken));
        }
    }
}
