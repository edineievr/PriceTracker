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

        public async Task<ProductTrackingResult> ExtractPriceAsync(string url)
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
                await page.WaitForSelectorAsync("#valVista", new() { Timeout = 5000 });

                var spotPriceText = await page.Locator("#valVista").First.TextContentAsync() ?? throw new Exception("Não foi possível extrair o preço à vista.");

                spotPriceText = spotPriceText.Replace("R$", "").Replace(".", "").Trim();

                if (!decimal.TryParse(spotPriceText, _currencyStyle, _culture, out decimal spotPrice))
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
                    var originalPriceText = await page.Locator("p.precode del").TextContentAsync();
                    originalPriceText = originalPriceText?.Replace("R$", "").Replace(".", "").Trim();

                    if (decimal.TryParse(originalPriceText, _currencyStyle, _culture, out decimal originalPrice))
                        result.OriginalPrice = originalPrice;
                }
                catch
                {
                    _logger.LogWarning("Preço original não encontrado na Terabyte. URL: {url}", url);
                }

                // parcelas
                try
                {
                    await page.WaitForSelectorAsync("#valParc", new() { Timeout = 5000 });

                    var installmentPriceText = await page.Locator("#valParc").TextContentAsync();
                    var installmentsText = await page.Locator("#nParc").TextContentAsync();

                    installmentPriceText = installmentPriceText?.Replace("R$", "").Replace(".", "").Trim();
                    var installments = int.Parse(new string(installmentsText?.Where(char.IsDigit).ToArray()));

                    if (decimal.TryParse(installmentPriceText, _currencyStyle, _culture, out decimal installmentPrice))
                    {
                        result.CreditCardPrice = installmentPrice * installments;
                        result.CreditCardInstallment = installments;
                    }
                }
                catch
                {
                    _logger.LogWarning("Parcelas não encontradas na Terabyte. URL: {url}", url);
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
