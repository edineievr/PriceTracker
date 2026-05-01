using AngleSharp;
using PriceTracker.DTOs;
using PriceTracker.Intefaces;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PriceTracker.Strategies
{
    public class MeliStrategy : IPriceScraper
    {
        public async Task<ProductTrackingResult> ExtractPrice(string url)
        {
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(url);

                var result = new ProductTrackingResult
                {
                    Platform = Platform.Meli,
                };                
                
                var titleElement = document.QuerySelector("h1.ui-pdp-title");//extrai o nome do produto atraves do titulo no site do meli
                
                if (titleElement != null)
                {
                    result.ProductDescription = titleElement.TextContent.Trim();
                }
                else
                {
                    result.ProductDescription = "Não foi possível extrair o nome, verificar metodo de extração";
                }                
                
                var priceContainer = document.QuerySelector("div.ui-pdp-price__second-line");//extrai o preço do produto atraves do container onde o preço é exibido no site do meli

                if (priceContainer != null)
                {                    
                    var fractionElement = priceContainer.QuerySelector("span[data-andes-money-amount-fraction]");
                    var centsElement = priceContainer.QuerySelector("span[data-andes-money-amount-cents]");

                    if (fractionElement != null && centsElement != null)
                    {
                        var fraction = fractionElement.TextContent.Trim();//pega numero à esquerda da vírgula
                        var cents = centsElement.TextContent.Trim(); //pega numero à direita da vírgula  

                        var priceString = $"{fraction},{cents}";
                        result.Price = decimal.Parse(priceString, CultureInfo.InvariantCulture);
                    }                  
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair preço do Mercado Livre: {ex.Message}");
                throw;
            }
        }
    }
}

