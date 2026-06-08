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
    public class GetProductsByIdQuery : IGetProductsByIdQuery
    {
        private readonly IDynamoDBContext _context;
        private readonly string _tableName;
        private readonly IAsyncPolicy _policy;

        public GetProductsByIdQuery(IDynamoDBContext context, IOptions<AwsSettings> settings, IAsyncPolicy policy)
        {
            _context = context;
            _tableName = settings.Value.ProductsTableName;
            _policy = policy;
        }

        public async Task<Product> ExecuteAsync(string id)
        {
            var loadConfig = new LoadConfig
            {
                OverrideTableName = _tableName,
            };

            return await _policy.ExecuteAsync(async () =>
            {
                var item = await _context.LoadAsync<ProductItem>(id, "METADATA", loadConfig);
                return item.Adapt<Product>();
            });
        }
    }
}
