using AngleSharp;
using PriceTracker.Worker.DTOs;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Intefaces;
using System.Globalization;

namespace PriceTracker.Worker.Strategies
{
    public class KabumStrategy : IPriceScraper
    {
        private readonly ILogger<KabumStrategy> _logger;
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");
        private static readonly NumberStyles CurrencyStyle = NumberStyles.Currency;

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

                var spotPriceElement = document.QuerySelector("h4.duration-500") ?? throw new Exception("Não foi possível extrair o preço do produto.");

                var originalPriceElement = document.QuerySelector("span.text-black-600.text-xs.font-normal.line-through");

                var creditCardPriceElement = document.QuerySelector("b.text-xs.font-bold.text-black-700");

                var creditCardInstallmentElement = document.QuerySelector("b.text-xs.font-normal.text-black-700");

                var creditCardPriceText = creditCardPriceElement?.TextContent.Replace("R$", "").Trim();

                var originalPriceText = originalPriceElement?.TextContent.Replace("R$", "").Trim();

                result.ProductDescription = titleElement.TextContent.Trim();

                var priceText = spotPriceElement.TextContent.Trim();

                priceText = priceText.Replace("R$", "").Trim();

                var creditCardInstallmentText = new string(creditCardInstallmentElement?.TextContent.Where(char.IsDigit).ToArray());

                if (!decimal.TryParse(priceText, CurrencyStyle, PtBr, out decimal price))
                {
                    throw new Exception("Não foi possível converter o preço do produto.");
                }                

                if (!decimal.TryParse(creditCardPriceText, CurrencyStyle, PtBr, out decimal creditCardPrice))
                {
                    _logger.LogWarning("Não foi possível converter o preço do cartão de crédito para o produto na Kabum. URL: {Url}", url);
                }
                else
                {
                    result.CreditCardPrice = creditCardPrice;
                }                

                if (!int.TryParse(creditCardInstallmentText, out int creditCardInstallment))
                {
                    _logger.LogWarning("Não foi possível converter o número de parcelas do cartão de crédito para o produto na Kabum. URL: {Url}", url);
                }
                else
                {
                    result.CreditCardInstallment = creditCardInstallment;
                }

                if (!decimal.TryParse(originalPriceText, CurrencyStyle, PtBr, out decimal originalPrice))
                {
                    _logger.LogWarning("Não foi possível converter o preço original do produto na Kabum. URL: {Url}", url);
                }
                else
                {
                    result.OriginalPrice = originalPrice;
                }

                result.SpotPrice = price;

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
