using PriceTracker.Worker.Intefaces;
using PriceTracker.Worker.Models;
using System.Net.Http.Json;

namespace PriceTracker.Worker.Services
{
    public class NotificationService : INotificationService
    {
        private readonly string _apiKey;
        private readonly string _chatId;

        public NotificationService()
        {           
            _apiKey = Environment.GetEnvironmentVariable("TELEGRAM_API_KEY");
            _chatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID");
        }
        public async Task NotifyAsync(PriceAlert priceAlert)
        {

            var httpClient = new HttpClient();            

            var payload = new
            {
                chat_id = _chatId,
                text = $"Alerta de Preço\nProduto: {priceAlert.ProductDescription}\nPreço Atual: {priceAlert.CurrentPrice}\nPlataforma: {priceAlert.Platform}",
                parse_mode = "Markdown"
            };

            var response = await httpClient.PostAsJsonAsync($"https://api.telegram.org/bot{_apiKey}/sendMessage",payload);

            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
    }
}
