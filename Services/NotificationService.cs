using PriceTracker.DTOs;
using PriceTracker.Models;
using static System.Net.WebRequestMethods;

namespace PriceTracker.Services
{
    public class NotificationService
    {
        private readonly string _apiKey;
        private readonly string _chatId;

        public NotificationService()
        {
            DotNetEnv.Env.Load();//mover isso aqui pro program quando o job for implementado
            _apiKey = Environment.GetEnvironmentVariable("TELEGRAM_API_KEY");
            _chatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID");
        }
        public async Task Notify(PriceAlert priceAlert)
        {

            var httpClient = new HttpClient();            

            var payload = new
            {
                chat_id = _chatId,
                text = $"Alerta de Preço\nProduto: {priceAlert.ProductDescription}\nPreço Atual: {priceAlert.CurrentPrice}\nPlataforma: {priceAlert.Platform.ToString()}",
                parse_mode = "Markdown"
            };

            var response = await httpClient.PostAsJsonAsync($"https://api.telegram.org/bot{_apiKey}/sendMessage",payload);

            Console.WriteLine(await response.Content.ReadAsStringAsync());//nao faço ideia do que isso ta fazendo
        }
    }
}
