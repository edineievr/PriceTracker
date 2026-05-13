using AngleSharp;
using Microsoft.Playwright;
using PriceTracker.Worker.DTOs;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Intefaces;
using Superpower.Model;
using System.Globalization;

namespace PriceTracker.Worker.Strategies
{
    public class MeliStrategy : IPriceScraper
    {
        private readonly ILogger<MeliStrategy> _logger;
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");
        private static readonly NumberStyles CurrencyStyle = NumberStyles.Currency;

        public MeliStrategy(ILogger<MeliStrategy> logger)
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

                var ogTitle = await page.Locator("meta[property='og:title']").GetAttributeAsync("content") ?? throw new Exception("Não foi possível extrair o título do produto.");

                var parts = ogTitle.Split(" - R$ ");
                var title = parts[0].Trim();
                var priceText = parts.Length > 1 ? parts[1].Trim() : throw new Exception("Não foi possível extrair o preço do produto.");

                if (!decimal.TryParse(priceText, CurrencyStyle, PtBr, out decimal price))
                    throw new Exception("Não foi possível converter o preço do produto.");

                var result = new ProductTrackingResult
                {
                    Platform = Platform.Meli,
                    ProductDescription = title,
                    SpotPrice = price
                };

                // preço original
                try
                {
                    await page.WaitForSelectorAsync("span.ui-pdp-price__part__container", new() { Timeout = 5000 });
                    var originalPriceElement = await page.QuerySelectorAsync("span.ui-pdp-price__part__container");

                    if (originalPriceElement != null)
                    {
                        var originalPriceText = await originalPriceElement.TextContentAsync();
                        originalPriceText = originalPriceText?.Replace("R$", "").Replace(".", "").Trim();

                        if (decimal.TryParse(originalPriceText, CurrencyStyle, PtBr, out decimal originalPrice))
                            result.OriginalPrice = originalPrice;
                    }
                }
                catch
                {
                    _logger.LogWarning("Preço original não encontrado para o produto no Meli. URL: {Url}", url);
                }

                // preço e parcelas no cartão
                try
                {
                    await page.WaitForSelectorAsync("div.ui-pdp-price__subtitles", new() { Timeout = 5000 });
                    var subtitlesElement = await page.QuerySelectorAsync("div.ui-pdp-price__subtitles");

                    if (subtitlesElement != null)
                    {
                        var subtitlesText = await subtitlesElement.TextContentAsync();
                        var numbers = subtitlesText?.Split('x');

                        if (numbers?.Length >= 2)
                        {
                            var installments = int.Parse(new string(numbers[0].Where(char.IsDigit).ToArray()));
                            var installmentPriceText = numbers[1].Replace("R$", "").Trim();

                            if (decimal.TryParse(installmentPriceText, CurrencyStyle, PtBr, out decimal installmentPrice))
                            {
                                result.CreditCardPrice = installmentPrice * installments;
                                result.CreditCardInstallment = installments;
                            }
                        }
                    }
                }
                catch
                {
                    _logger.LogWarning("Informações de parcelamento não encontradas para o produto no Meli. URL: {Url}", url);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair preço do produto no Meli. URL: {Url}", url);
                throw;
            }
        }
    }
}

