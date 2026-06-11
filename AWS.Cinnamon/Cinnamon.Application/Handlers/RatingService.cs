using Cinnamon.Application.Common;
using Cinnamon.Application.Interfaces;
using Cinnamon.Application.Interfaces.Actions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Application.Handlers
{
    public class RatingService(IRatingPublisher _ratingPublisher): IRatingService
    {
        public async Task RateProduct(string productId, int rating, RatingMessageType type)
        {
            await _ratingPublisher.PublishAsync(new RatingMessage
            {
                ProductId = productId,
                Rating = rating.ToString(),
                MessageType = type
            });
        }
    }
}
