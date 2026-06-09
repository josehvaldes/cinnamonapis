using Asp.Versioning;
using AWS.Cinnamon.Api.Services;
using Cinnamon.Application.Interfaces;
using Cinnamon.Contracts.Requests;
using Cinnamon.Contracts.Responses;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace AWS.Cinnamon.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class ProductsController(ILogger<ProductsController> logger, 
        IHandler _handler, 
        ILinkService linkService) : ControllerBase
    {

        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "Public5min")]
        public async Task<IActionResult> GetProduct(string id)
        {
            logger.LogInformation("Received request for product with ID: {ProductId}", id);
            var product = await _handler.GetProductById(id);
            var productResponse = product.Adapt<ProductResponse>();
            return Ok(productResponse);
        }

        [HttpGet("category/{category}")]
        [ResponseCache(CacheProfileName = "Public5min")]
        public async Task<IActionResult> GetProductsByCategory(string category, [FromQuery] GetPagingRequest request)
        {
            var pagedResult = await _handler.GetProductsByCategory(category, request.PageNumber, request.PageSize);
            var items = pagedResult.Items.Adapt<List<ProductResponse>>();
            foreach (var item in items)
                item.Links = linkService.GetProductLinks(item.Id);
            var response = new PagedResponse<ProductResponse>(
                items,
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize,
                pagedResult.TotalPages,
                pagedResult.HasNextPage,
                pagedResult.HasPreviousPage);
            return Ok(response);
        }
    }
}
