using Amazon.DynamoDBv2.DataModel;
using Cinnamon.Application.Interfaces.Queries;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.AWS.Model;
using Cinnamon.Infrastructure.AWS.Settings;
using Mapster;
using Microsoft.Extensions.Options;
using Polly;

namespace Cinnamon.Infrastructure.AWS.Queries
{
    public class GetNewArrivalsQuery : IGetNewArrivalsQuery
    {
        private readonly IAsyncPolicy _policy;
        private readonly IDynamoDBContext _context;
        private readonly string _productsTableName;
        private readonly string _mappingsTableName;

        public GetNewArrivalsQuery(IDynamoDBContext context, IOptions<AwsSettings> settings, IAsyncPolicy policy)
        {
            _context = context;
            _productsTableName = settings.Value.ProductsTableName;
            _mappingsTableName = settings.Value.MappingsTableName;
            _policy = policy;
        }

        public async Task<List<Product>> ExecuteAsync()
        {
            var mappingQueryConfig = new QueryConfig { OverrideTableName = _mappingsTableName };
            var mappings = await _policy.ExecuteAsync(async () =>
            {
                var items = await _context.QueryAsync<MappingItem>("CATEGORY#new-arrivals", mappingQueryConfig).GetRemainingAsync();
                return items;
            });


            if (mappings == null || !mappings.Any())
            {
                return new List<Product>();
            }

            var queryConfig = new BatchGetConfig { OverrideTableName = _productsTableName };
            var getbatch = _context.CreateBatchGet<ProductItem>(queryConfig);
            foreach (var mapping in mappings)
            {
                getbatch.AddKey(mapping.Value, "METADATA");
            }
            return await _policy.ExecuteAsync(async () =>
            {
                await getbatch.ExecuteAsync();
                return getbatch.Results.Adapt<List<Product>>();
            });
        }
    }
}
