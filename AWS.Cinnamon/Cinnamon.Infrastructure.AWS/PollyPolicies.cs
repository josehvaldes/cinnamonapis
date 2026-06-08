using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace Cinnamon.Infrastructure.AWS
{
    public static class PollyPolicies
    {
        public static AsyncRetryPolicy GetRetryPolicy()
        {
            return Policy
                // Throttling: DynamoDB returns HTTP 400, not 429
                .Handle<ProvisionedThroughputExceededException>()
                .Or<RequestLimitExceededException>()
                // Transient server-side failures
                .Or<AmazonDynamoDBException>(ex =>
                    ex.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                    ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                // Network/client-side failures
                .Or<AmazonClientException>()
                .Or<System.Net.Http.HttpRequestException>()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                  + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000)) // jitter
                );
        }

        public static AsyncCircuitBreakerPolicy GetCircuitBreakerPolicy()
        {
            return Policy
                // Only trip the circuit on transient infrastructure failures, not on client errors
                .Handle<ProvisionedThroughputExceededException>()
                .Or<RequestLimitExceededException>()
                .Or<AmazonDynamoDBException>(ex =>
                    ex.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                    ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                .Or<AmazonClientException>()
                .Or<System.Net.Http.HttpRequestException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );
        }
    }
}
