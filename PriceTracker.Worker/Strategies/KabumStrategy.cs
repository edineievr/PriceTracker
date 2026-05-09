using AngleSharp;
using PriceTracker.Worker.DTOs;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Intefaces;

namespace PriceTracker.Worker.Strategies
{
    public class KabumStrategy : IPriceScraper
    {
        private readonly ILogger<KabumStrategy> _logger;

        public KabumStrategy(ILogger<KabumStrategy> logger)
        {
            _logger = logger;
        }

        public async Task<ProductTrackingResult> ExtractPriceAsync(string url)
        {
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(url);

                var result = new ProductTrackingResult
                {
                    Platform = Platform.Kabum,
                };

                var titleElement = document.QuerySelector("h1.text-black-800") ?? throw new Exception("Não foi possível extrair o título do produto.");

                var priceContainer = document.QuerySelector("h4.duration-500") ?? throw new Exception("Não foi possível extrair o preço do produto.");

                result.ProductDescription = titleElement.TextContent.Trim();

                var priceText = priceContainer.TextContent.Trim();
                priceText = priceText.Replace("R$", "").Trim();

                if (!decimal.TryParse(priceText, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out decimal price))
                    throw new Exception("Não foi possível converter o preço do produto.");

                result.Price = price;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair preço do produto na Kabum. URL: {Url}", url);
                throw;
            }
        }
    }
}
