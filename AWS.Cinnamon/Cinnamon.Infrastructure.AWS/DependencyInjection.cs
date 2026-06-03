using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Cinnamon.Application.Interfaces.Queries;
using Cinnamon.Infrastructure.AWS.Queries;
using Cinnamon.Infrastructure.AWS.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddScoped<IGetNewArrivalsQuery, GetNewArrivalsQuery>();
            services.AddScoped<IGetTrendingQuery, GetTrendingQuery>();
            services.AddScoped<IGetOnSalesQuery, GetOnSalesQuery>();
            services.AddScoped<IGetProductsByIdQuery, GetProductsByIdQuery>();
            services.AddScoped<IGetProductByCategoryAndIdQuery, GetProductByCategoryAndIdQuery>();
            services.AddScoped<IGetProductsByCategoryAndInStock, GetProductsByCategoryAndInStock>();

            return services;
        }
    }
}
