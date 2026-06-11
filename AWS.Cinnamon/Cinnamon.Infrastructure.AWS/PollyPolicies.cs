using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.SQS;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace Cinnamon.Infrastructure.AWS
{
    public static class PollyPolicies
    {
        public static IAsyncPolicy GetDynamoDBPolicy() 
        {
            var retry = PollyPolicies.GetRetryPolicy();
            var breaker = PollyPolicies.GetCircuitBreakerPolicy();
            return Policy.WrapAsync(breaker, retry); // breaker outer: trips once after all retries fail
        }

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

        public static IAsyncPolicy GetSqsPolicy()
        {
            var retry = Policy
                // Throttling and transient server-side failures
                .Handle<AmazonSQSException>(ex =>
                    ex.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                    ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable ||
                    ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                // Network/client-side failures
                .Or<AmazonClientException>()
                .Or<System.Net.Http.HttpRequestException>()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                  + TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000))
                );

            var breaker = Policy
                .Handle<AmazonSQSException>(ex =>
                    ex.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                    ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                .Or<AmazonClientException>()
                .Or<System.Net.Http.HttpRequestException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );

            return Policy.WrapAsync(breaker, retry);
        }
    }
}
