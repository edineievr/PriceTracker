using Microsoft.Playwright;
using PriceTracker.Worker.DTOs;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Intefaces;
using System.Globalization;

namespace PriceTracker.Worker.Strategies
{
    public class TerabyteStrategy : IPriceScraper
    {
        private readonly ILogger<TerabyteStrategy> _logger;
        private static readonly CultureInfo _culture = CultureInfo.InvariantCulture;
        private static readonly NumberStyles _currencyStyle = NumberStyles.Currency;

        public TerabyteStrategy(ILogger<TerabyteStrategy> logger)
        {
            _logger = logger;
        }

        public async Task<ProductTrackingResult> ExtractPriceAsync(string url, CancellationToken stoppingToken)
        {
            try
            {
                using var playwright = await Playwright.CreateAsync();

                await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

                var page = await browser.NewContextAsync(new()
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"
                }).Result.NewPageAsync();

                await page.GotoAsync(url, new() { WaitUntil = WaitUntilState.DOMContentLoaded });

                // título
                await page.WaitForSelectorAsync("h1.tit-prod", new() { Timeout = 5000 });
                var titleElement = await page.Locator("h1.tit-prod").First.TextContentAsync();

                var title = !string.IsNullOrEmpty(titleElement) ? titleElement.Trim() :
                            await page.Locator("meta[property='og:title']").GetAttributeAsync("content") ??
                            throw new Exception("Não foi possível extrair o título do produto.");

                // preço à vista — id estável, mais resiliente que classe
                await page.WaitForSelectorAsync("p#valVista.val-prod.valVista", new() { Timeout = 5000 });

                var spotPriceText = await page.Locator("p#valVista.val-prod.valVista").First.TextContentAsync() ?? throw new Exception("Não foi possível extrair o preço à vista.");

                spotPriceText = spotPriceText.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim();

                if (!decimal.TryParse(spotPriceText, _currencyStyle, CultureInfo.InvariantCulture, out decimal spotPrice))
                    throw new Exception("Não foi possível converter o preço à vista.");

                var result = new ProductTrackingResult
                {
                    Platform = Platform.Terabyte,
                    ProductDescription = title,
                    SpotPrice = spotPrice
                };

                // preço original
                try
                {
                    await page.WaitForSelectorAsync("p.precode del", new() { Timeout = 5000 });
                    var originalPriceText = await page.Locator("p.precode del").First.TextContentAsync();
                    originalPriceText = originalPriceText?.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim();

                    if (decimal.TryParse(originalPriceText, _currencyStyle, _culture, out decimal originalPrice))
                        result.OriginalPrice = originalPrice;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Preço original não encontrado na Terabyte. URL: {url}", url);
                }

                // preço cartão
                try
                {
                    var creditCardPriceText = await page.Locator("span#valParc").First.TextContentAsync(new() { Timeout = 5000 });
                    creditCardPriceText = creditCardPriceText?.Replace("R$", "").Replace(".", "").Replace(",", ".").Trim();

                    if (decimal.TryParse(creditCardPriceText, _currencyStyle, _culture, out decimal creditCardPrice))
                        result.CreditCardPrice = creditCardPrice;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Preço no cartão de crédito não encontrado na Terabyte. URL: {url}", url);
                }

                // parcelas
                try
                {
                    var installmentsText = await page.Locator("span#nParc").First.TextContentAsync(new() { Timeout = 5000 });
                    var installments = int.Parse(new string(installmentsText?.Where(char.IsDigit).ToArray()));

                    result.CreditCardInstallment = installments;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Parcelas não encontradas na Terabyte. URL: {url}", url);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair preço do produto na Terabyte. URL: {url}", url);
                throw;
            }
        }
    }
}
