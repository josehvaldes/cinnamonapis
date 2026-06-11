using Asp.Versioning;
using AWS.Cinnamon.Api.Services;
using Cinnamon.Application.Common;
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
        IProductService _handler,
        IRatingService _ratingService,
        ILinkService linkService) : ControllerBase
    {

        [HttpGet("{id}")]
        [ResponseCache(CacheProfileName = "Public5min")]
        public async Task<IActionResult> GetProduct(string id)
        {
            logger.LogInformation("Received request for product with ID: {ProductId}", id);
            var product = await _handler.GetProductById(id);
            var productResponse = product.Adapt<ProductResponse>();
            productResponse.Links = linkService.GetProductLinks(productResponse.Id);
            logger.LogInformation("Returning product with ID: {ProductId}", id);
            return Ok(productResponse);
        }

        [HttpGet("category/{category}")]
        [ResponseCache(CacheProfileName = "Public5min")]
        public async Task<IActionResult> GetProductsByCategory(string category, [FromQuery] GetPagingRequest request)
        {
            logger.LogInformation("Received request for products in category: {Category} with page number: {PageNumber} and page size: {PageSize}", category, request.PageNumber, request.PageSize);

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

            logger.LogInformation("Returning {Count} products for category: {Category} with page number: {PageNumber} and page size: {PageSize}", items.Count, category, request.PageNumber, request.PageSize);
            return Ok(response);
        }

        [HttpPost("{id}/rate")]
        public async Task<IActionResult> RateProduct(string id, [FromBody] RateProductRequest request)
        {
            logger.LogInformation("Received request to rate product with ID: {ProductId} with rating: {Rating} and like status: {LikeIt}", id, request.Value, request.RatingType);
            await _ratingService.RateProduct(id, request.Value, request.RatingType == "like" ? RatingMessageType.Like : RatingMessageType.Rating);
            logger.LogInformation("Successfully rated product with ID: {ProductId} with rating: {Rating} and like status: {LikeIt}", id, request.Value, request.RatingType  );
            return Accepted();
        }

    }
}
