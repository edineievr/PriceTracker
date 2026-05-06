using AngleSharp;
using PriceTracker.Worker.DTOs;
using PriceTracker.Worker.Enums;
using PriceTracker.Worker.Intefaces;

namespace PriceTracker.Worker.Strategies
{
    public class KabumStrategy : IPriceScraper
    {
        public async Task<ProductTrackingResult> ExtractPriceAsync(string url)//verificar um meio de otimizar isso aqui, ta feio
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);

            var result = new ProductTrackingResult
            {
                Platform = Platform.Kabum,
            };

            var titleElement = document.QuerySelector("h1.text-black-800");

            if (titleElement != null)
            {
                result.ProductDescription = titleElement.TextContent.Trim();
            }
            else
            {
                result.ProductDescription = "Descrição do produto não encontrada";
            }

            var priceContainer = document.QuerySelector("h4.duration-500");//extrai o preço do produto no site da kabum

            if (priceContainer != null)
            {                

                var priceText = priceContainer.TextContent.Trim();

                priceText = priceText.Replace("R$", "").Trim();

                if (decimal.TryParse(priceText, System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out decimal price))
                {
                    result.Price = price;
                }
                else
                {
                    result.Price = 0; //0 indica que o preço não foi encontrado ou não pôde ser convertido
                }
            }
            else
            {
                result.Price = 0; //0 indica que o preço não foi encontrado ou não pôde ser convertido
            }

            return result;

        }
    }
}
