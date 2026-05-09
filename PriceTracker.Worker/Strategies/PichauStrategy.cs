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

                if (!decimal.TryParse(priceText, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal price))
                    throw new Exception("Não foi possível converter o preço do produto.");

                return new ProductTrackingResult
                {
                    Platform = Platform.Pichau,
                    ProductDescription = title.Replace("| Pichau", "").Trim(),
                    Price = price
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair preço do produto na Pichau. URL: {url}", url);
                throw;
            }
        }
    }
}
