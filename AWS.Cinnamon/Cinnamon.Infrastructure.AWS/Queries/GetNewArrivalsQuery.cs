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
        private readonly string _tableName;

        public GetNewArrivalsQuery(IDynamoDBContext context, IOptions<AwsSettings> settings, IAsyncPolicy policy)
        {
            _context = context;
            _tableName = settings.Value.DynamoDbTableName;
            _policy = policy;
        }

        public async Task<List<Product>> ExecuteAsync()
        {
            var queryConfig = new QueryConfig { OverrideTableName = _tableName };
            return await _policy.ExecuteAsync(async () =>
            {
                var items = await _context.QueryAsync<ProductItem>("new-arrivals", queryConfig).GetRemainingAsync();
                return items.Adapt<List<Product>>();
            });
        }
    }
}
