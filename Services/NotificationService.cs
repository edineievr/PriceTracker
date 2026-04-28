using PriceTracker.DTOs;
using static System.Net.WebRequestMethods;

namespace PriceTracker.Services
{
    public class NotificationService
    {
        private readonly string _apiKey;
        private readonly string _chatId;

        public NotificationService()
        {
            DotNetEnv.Env.Load();
            _apiKey = Environment.GetEnvironmentVariable("TELEGRAM_API_KEY");
            _chatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID");
        }
        public async Task SendNotification(PriceAlertDto priceAlert)
        {

            var httpClient = new HttpClient();            

            var payload = new
            {
                chat_id = _chatId,
                text = $"Alerta de Preço: {priceAlert.ProductDescription}\nPreço Atual: {priceAlert.CurrentPrice}\nPlataforma: {priceAlert.Platform}",
                parse_mode = "Markdown"
            };

            var response = await httpClient.PostAsJsonAsync($"https://api.telegram.org/bot{_apiKey}/sendMessage",payload);

            Console.WriteLine(await response.Content.ReadAsStringAsync());//nao faço ideia do que isso ta fazendo
        }
    }
}
