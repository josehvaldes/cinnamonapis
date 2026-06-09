using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Cinnamon.Application.Interfaces.Queries;
using Cinnamon.Infrastructure.AWS.Queries;
using Cinnamon.Infrastructure.AWS.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Cinnamon.Infrastructure.AWS
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAWSDependencies(
                    this IServiceCollection services, IConfiguration config)
        {
            AwsMapping.RegisterMappings();

            services.Configure<AwsSettings>(config.GetSection("AWS"));
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddPollyDependencies(config);
            services.AddSingleton<IDynamoDBContext>(sp =>
            {
                var client = sp.GetRequiredService<IAmazonDynamoDB>();

                return new DynamoDBContextBuilder()
                    .WithDynamoDBClient(() => client)
                    .ConfigureContext(config =>
                    {
                        config.DisableFetchingTableMetadata = true;
                        config.Conversion = DynamoDBEntryConversion.V2;
                    })
                    .Build();
            });

            services.AddScoped<IGetProductsByIdQuery, GetProductsByIdQuery>();
            services.AddScoped<IGetProductsByCategoryQuery, GetProductsByCategoryQuery>();

            return services;
        }

        public static IServiceCollection AddPollyDependencies(this IServiceCollection services, IConfiguration config) 
        {
            services.AddSingleton<IAsyncPolicy>(sp =>
            {
                var retry = PollyPolicies.GetRetryPolicy();
                var breaker = PollyPolicies.GetCircuitBreakerPolicy();

                return Policy.WrapAsync(breaker, retry); // breaker outer: trips once after all retries fail
            });
            return services;
        }
    }
}
