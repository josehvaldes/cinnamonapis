using Cinnamon.Application.Interfaces;
using Cinnamon.Contracts.Responses.Homepage;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AWS.Cinnamon.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomepageController(ILogger<HomepageController> _logger,
        IHandler _handler) : ControllerBase
    {
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Received request for homepage data.");


            var trendingProducts = await _handler.GetTrending();
            var newArrivals = await _handler.GetNewArrivals();
            var onsales = await _handler.GetOnSales();

            var homepagedata = new HomepageResponse
            {
                TrendingProducts = trendingProducts.Adapt<IReadOnlyList<ProductResponse>>(),
                NewArrivals = newArrivals.Adapt<IReadOnlyList<ProductResponse>>(),
                OnSales = onsales.Adapt<IReadOnlyList<ProductResponse>>()
            };

            return Ok(homepagedata);
        }
    }
}
