using Asp.Versioning;
using Cinnamon.Application.Interfaces;
using Cinnamon.Contracts.Responses.Homepage;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWS.Cinnamon.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class ProductsController(ILogger<ProductsController> logger, IHandler _handler) : ControllerBase
    {

        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "Public5min")]
        public async Task<IActionResult> GetById(string id)
        {
            logger.LogInformation("Received request for product with ID: {ProductId}", id);
            var product = await _handler.GetProductById(id);
            var productResponse = product.Adapt<ProductResponse>();
            return Ok(productResponse);
        }
    }
}
