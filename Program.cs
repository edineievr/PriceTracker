using PriceTracker.Strategies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<MeliPriceScraper>();
Console.WriteLine("executou");

var app = builder.Build();

var priceChecker = new MeliPriceScraper();

var lista = await priceChecker.ExtractPrice("https://produto.mercadolivre.com.br/MLB-1748426123-kit-10-cuecas-boxer-sandrini-masculinas-algodo-conforto-_JM?searchVariation=71283718056#polycard_client=search-desktop&searchVariation=71283718056&search_layout=grid&position=10&type=item&tracking_id=6e5bf45e-d081-4ae8-9a84-699332523916");

Console.WriteLine(lista.ProductDescription);
Console.WriteLine(lista.Price);

// Exemplo: extrair preços de um site
// await priceChecker.ExtractPricesAsync("https://seu-site.com");

// Exemplo: extrair detalhes de produtos
// await priceChecker.ExtractProductDetailsAsync("https://seu-site.com");


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


