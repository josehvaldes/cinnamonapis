using Asp.Versioning;
using Cinnamon.Application.Interfaces;
using Cinnamon.Contracts.Responses;
using Cinnamon.Contracts.Responses.Homepage;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWS.Cinnamon.Api.Controllers
{    
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class HomepageController(ILogger<HomepageController> _logger,
        IHandler _handler) : ControllerBase
    {
        [HttpGet]
        [ResponseCache(CacheProfileName ="Public5min")]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Received request for homepage data.");


            var trendingProducts = await _handler.GetTrending();
            var newArrivals = await _handler.GetNewArrivals();
            var onsales = await _handler.GetOnSales();

            var homepagedata = new HomepageResponse
            {
                Trendings = trendingProducts.Adapt<IReadOnlyList<ProductResponse>>(),
                NewArrivals = newArrivals.Adapt<IReadOnlyList<ProductResponse>>(),
                OnSales = onsales.Adapt<IReadOnlyList<ProductResponse>>()
            };
            _logger.LogInformation("Returning homepage data with {TrendingCount} trending products, {NewArrivalCount} new arrivals, and {OnSaleCount} on-sale products.", homepagedata.Trendings.Count, homepagedata.NewArrivals.Count, homepagedata.OnSales.Count);
            return Ok(homepagedata);
        }
    }
}
