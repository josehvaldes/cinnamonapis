using Cinnamon.Application.Common;

namespace Cinnamon.Application.Interfaces.Actions
{
    public interface IRatingPublisher
    {
        Task PublishAsync(RatingMessage ratingMessage);
    }
}
