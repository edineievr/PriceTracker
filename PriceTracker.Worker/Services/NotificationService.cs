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
        public async Task NotifyAsync(PriceAlert priceAlert)
        {
            _logger.LogInformation("Enviando notificação...{productDescription}", priceAlert.ProductDescription);                 

            var payload = new
            {
                chat_id = _chatId,
                text = $"Alerta de Preço\nProduto: {priceAlert.ProductDescription}\nPreço Atual: {priceAlert.CurrentPrice}\nPlataforma: {priceAlert.Platform}",
                parse_mode = "Markdown"
            };

            var response = await _httpClient.PostAsJsonAsync($"https://api.telegram.org/bot{_apiKey}/sendMessage",payload);

            _logger.LogInformation("Notificação enviada!");

            _logger.LogWarning("Resposta da API: {response}", await response.Content.ReadAsStringAsync());
        }
    }
}
