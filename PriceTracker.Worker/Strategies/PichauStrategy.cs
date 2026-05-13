using AngleSharp;
using Microsoft.Playwright;
using PriceTracker.Worker.DTOs;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Intefaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PriceTracker.Worker.Strategies
{
    public class PichauStrategy : IPriceScraper
    {
        private readonly ILogger<PichauStrategy> _logger;
        private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");
        private static readonly NumberStyles CurrencyStyle = NumberStyles.Currency;

        public PichauStrategy(ILogger<PichauStrategy> logger)
        {
            _logger = logger;
        }
        public async Task<ProductTrackingResult> ExtractPriceAsync(string url)
        {
            try
            {
                using var playwright = await Playwright.CreateAsync();

                await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

                // Cria uma nova página dentro do browser com User-Agent de browser real
                // sem isso a Pichau identifica que não é humano e serve página de bloqueio
                var page = await browser.NewContextAsync(new()
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"
                }).Result.NewPageAsync();

                // Navega até a URL e espera o HTML inicial carregar
                // DOMContentLoaded é suficiente pq os meta tags chegam no HTML, antes do JS rodar
                await page.GotoAsync(url, new() { WaitUntil = WaitUntilState.DOMContentLoaded });

                var title = await page.Locator("meta[property='og:title']").GetAttributeAsync("content") ?? throw new Exception("Não foi possível extrair o título do produto.");

                var priceText = await page.Locator("meta[name='product:price:amount']").GetAttributeAsync("content") ?? throw new Exception("Não foi possível extrair o preço do produto.");

                priceText = priceText.Replace("R$", "").Trim();

                if (!decimal.TryParse(priceText, CurrencyStyle, PtBr, out decimal price))
                    throw new Exception("Não foi possível converter o preço do produto.");

                var result = new ProductTrackingResult
                {
                    Platform = Platform.Pichau,
                    ProductDescription = title.Replace("| Pichau", "").Trim(),
                    SpotPrice = price
                };

                // preço original
                try
                {
                    await page.WaitForSelectorAsync("span.mui-3ij2mi-strikeThrough", new() { Timeout = 5000 });
                    var originalPriceElement = await page.QuerySelectorAsync("span.mui-3ij2mi-strikeThrough");

                    if (originalPriceElement != null)
                    {
                        var originalPriceText = (await originalPriceElement.TextContentAsync())?.Replace("R$", "").Trim();

                        if (decimal.TryParse(originalPriceText, CurrencyStyle, PtBr, out decimal originalPrice))
                            result.OriginalPrice = originalPrice;
                    }
                }
                catch
                {
                    _logger.LogWarning("Preço original não encontrado na Pichau. URL: {url}", url);
                }

                // parcelas
                try
                {
                    await page.WaitForSelectorAsync("div.mui-1oz0vcv-installment", new() { Timeout = 5000 });
                    var installmentsElement = await page.QuerySelectorAsync("div.mui-1oz0vcv-installment");

                    if (installmentsElement != null)
                    {
                        var text = await installmentsElement.TextContentAsync();
                        var numbers = text?.Split('x');

                        if (numbers?.Length >= 2)
                        {
                            var installments = int.Parse(new string(numbers[0].Where(char.IsDigit).ToArray()));
                            var installmentPriceText = numbers[1].Replace("R$", "").Replace("de", "").Trim();

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
                    _logger.LogWarning("Parcelas não encontradas na Pichau. URL: {url}", url);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair preço do produto na Pichau. URL: {url}", url);
                throw;
            }
        }
    }
}
