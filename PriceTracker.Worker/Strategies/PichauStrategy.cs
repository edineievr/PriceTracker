using AngleSharp;
using PriceTracker.Worker.DTOs;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Intefaces;
using System;
using System.Collections.Generic;
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
                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(url);

                var result = new ProductTrackingResult
                {
                    Platform = Platform.Pichau,
                };

                var titleElement = document.QuerySelector("h1.MuiTypography-root") ?? throw new Exception("Não foi possível extrair o titulo do produto.");//não faz sentido continuar se eu nao extrai o titulo.;

                var priceContainer = document.QuerySelector("div.mui-1jk88bq-price_vista-extraSpacePriceVista") ?? throw new Exception("Não foi possível extrair o preço do produto.");

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
                _logger.LogError(ex, "Erro ao extrair preço do produto na Pichau. URL: {url}", url);
                throw;
            }
        }
    }
}
