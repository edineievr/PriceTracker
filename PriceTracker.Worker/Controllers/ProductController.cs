using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PriceTracker.Worker.DTOs;
using PriceTracker.Worker.Infrastructure;
using PriceTracker.Worker.Models;

namespace PriceTracker.Worker.Controllers
{
    [ApiController]
    [Route("products")]    
    public class ProductController(Database database) : ControllerBase
    {
        private readonly Database _database = database;

        [HttpPost]
        public async Task<IActionResult> InsertProduct([FromBody] AddProductRequest request)
        {
            var product = Product.Create(request.Description, request.Platform, request.Url);

            await _database.InsertProductAsync(product);

            return Created();
        }
    }
}
