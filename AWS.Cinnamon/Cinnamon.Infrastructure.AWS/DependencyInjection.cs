using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.SQS;
using Cinnamon.Application.Interfaces.Actions;
using Cinnamon.Application.Interfaces.Queries;
using Cinnamon.Infrastructure.AWS.Actions;
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
            services.AddAWSService<IAmazonSQS>();

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
            services.AddScoped<IRatingPublisher, RatingPublisher>();

            return services;
        }

        public static IServiceCollection AddPollyDependencies(this IServiceCollection services, IConfiguration config) 
        {
            services.AddSingleton<IAsyncPolicy>(sp =>PollyPolicies.GetDynamoDBPolicy());

            services.AddKeyedSingleton<IAsyncPolicy>("sqs", (_, _) => PollyPolicies.GetSqsPolicy());

            return services;
        }
    }
}
