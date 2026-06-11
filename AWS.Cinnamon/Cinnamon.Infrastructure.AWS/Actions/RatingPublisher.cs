using Amazon.SQS;
using Cinnamon.Application.Common;
using Cinnamon.Application.Interfaces.Actions;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.AWS.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Infrastructure.AWS.Actions
{
    public class RatingPublisher : IRatingPublisher
    {

        private readonly IAmazonSQS _sqsClient;
        private readonly string _sqsUrl;
        private readonly IAsyncPolicy _policy;

        public RatingPublisher(IAmazonSQS sqsClient, IOptions<AwsSettings> settings, [FromKeyedServices("sqs")] IAsyncPolicy policy)
        {
            _sqsClient = sqsClient;
            _sqsUrl = settings.Value.SQSQueueUrl;
            _policy = policy;
        }

        public async Task PublishAsync(RatingMessage ratingMessage)
        {
            var message = new 
            { 
                Rating = ratingMessage.Rating,
                ProductId = ratingMessage.ProductId,
                MessageType = ratingMessage.MessageType.ToString(),
                Timestamp = DateTime.UtcNow,
                Sender = "RatingService" // Customize it later
            };

            var request = new Amazon.SQS.Model.SendMessageRequest
            {
                QueueUrl = _sqsUrl,
                MessageBody = System.Text.Json.JsonSerializer.Serialize(message)
            };
            await _policy.ExecuteAsync(() => _sqsClient.SendMessageAsync(request));
        }
    }
}
