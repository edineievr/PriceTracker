//using AngleSharp;
//using PriceTracker.Worker.DTOs;
//using PriceTracker.Worker.Enums;
//using PriceTracker.Worker.Intefaces;
//using System.Globalization;

//namespace PriceTracker.Worker.Strategies
//{
//    public class MeliStrategy : IPriceScraper
//    {
//        private readonly ILogger<MeliStrategy> _logger;

//        public MeliStrategy(ILogger<MeliStrategy> logger)
//        {
//            _logger = logger;
//        }

//        public async Task<ProductTrackingResult> ExtractPriceAsync(string url)//temporariamente desativado, pois o site mudou a estrutura e precisa ser refeito o método de extração
//        {
//            try
//            {
//                var config = Configuration.Default.WithDefaultLoader();
//                var context = BrowsingContext.New(config);
//                var document = await context.OpenAsync(url);

//                var result = new ProductTrackingResult
//                {
//                    Platform = Platform.Meli,
//                };                
                
//                var titleElement = document.QuerySelector("h1.ui-pdp-title");
                
//                if (titleElement != null)
//                {
//                    result.ProductDescription = titleElement.TextContent.Trim();
//                }
//                else
//                {
//                    result.ProductDescription = "Não foi possível extrair o nome, verificar metodo de extração";
//                }                
                
//                var priceContainer = document.QuerySelector("div.ui-pdp-price__second-line");

//                if (priceContainer != null)
//                {                    
//                    var fractionElement = priceContainer.QuerySelector("span[data-andes-money-amount-fraction]");
//                    var centsElement = priceContainer.QuerySelector("span[data-andes-money-amount-cents]");

//                    if (fractionElement != null && centsElement != null)
//                    {
//                        var fraction = fractionElement.TextContent.Trim();
//                        var cents = centsElement.TextContent.Trim();
//                        var priceString = $"{fraction},{cents}";
//                        result.Price = decimal.Parse(priceString, CultureInfo.InvariantCulture);
//                    }                  
//                }

//                return result;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao extrair preço do produto no Meli. URL: {Url}", url);
//                throw;
//            }
//        }
//    }
//}

