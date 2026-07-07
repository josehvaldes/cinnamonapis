using Asp.Versioning;
using Cinnamon.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AWS.Cinnamon.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WishlistController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> PostWishlistItem([FromBody] AddWishlistRequest wishlistItems)
        {
            Console.WriteLine("Received wishlist items:");
            Console.WriteLine("Email: " + wishlistItems.Email);
            Console.WriteLine("Ids: " + string.Join(", ", wishlistItems.ProductIds));
            // Mock method to simulate adding wishlist items to a database or service
            return Accepted();
        }
    }
}
