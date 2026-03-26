using AngleSharp;
using PriceTracker.DTOs.ProductRequest;
using PriceTracker.Intefaces;
using System.Text.RegularExpressions;

namespace PriceTracker.Strategies
{
    public class MeliPriceScraper : IPriceScraper
    {
        public async Task<PriceAlertDto> ExtractPrice(string url)
        {
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(url);

                var priceAlert = new PriceAlertDto
                {
                    Platform = "Mercado Livre"
                };                
                
                var titleElement = document.QuerySelector("h1.ui-pdp-title");//extrai o nome do produto atraves do titulo no site do meli
                
                if (titleElement != null)
                {
                    priceAlert.ProductDescription = titleElement.TextContent.Trim();
                }
                else
                {
                    priceAlert.ProductDescription = "Não foi possível extrair o nome, verificar metodo de extração";
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
                        priceAlert.Price = decimal.Parse(priceString);
                    }                  
                }

                return priceAlert;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair preço do Mercado Livre: {ex.Message}");
                throw;
            }
        }
    }
}

