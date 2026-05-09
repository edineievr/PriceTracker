using AngleSharp;
using Microsoft.Playwright;
using PriceTracker.Worker.DTOs;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Intefaces;
using System.Globalization;

namespace PriceTracker.Worker.Strategies
{
    public class MeliStrategy : IPriceScraper
    {
        private readonly ILogger<MeliStrategy> _logger;

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

                // og:title vem como "Produto X - R$ 419,99", então separei pelo " - R$ "
                var ogTitle = await page.Locator("meta[property='og:title']").GetAttributeAsync("content") ?? throw new Exception("Não foi possível extrair o título do produto.");

                var parts = ogTitle.Split(" - R$ ");

                var title = parts[0].Trim();
                var priceText = parts.Length > 1 ? parts[1].Trim() : throw new Exception("Não foi possível extrair o preço do produto.");

                if (!decimal.TryParse(priceText, NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-BR"), out decimal price))
                    throw new Exception("Não foi possível converter o preço do produto.");

                return new ProductTrackingResult
                {
                    Platform = Platform.Meli,
                    ProductDescription = title,
                    Price = price
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair preço do produto no Meli. URL: {Url}", url);
                throw;
            }
        }
    }
}

