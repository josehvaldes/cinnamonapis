
using Cinnamon.Application.Common;

namespace Cinnamon.Application.Interfaces
{
    public interface IRatingService
    {
        Task RateProduct(string productId, int rating, RatingMessageType type);
    }
}
