using Amazon.DynamoDBv2.DataModel;
using Cinnamon.Application.Interfaces.Queries;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.AWS.Model;
using Cinnamon.Infrastructure.AWS.Settings;
using Mapster;
using Microsoft.Extensions.Options;

namespace Cinnamon.Infrastructure.AWS.Queries
{
    public class GetProductsByIdQuery : IGetProductsByIdQuery
    {
        private readonly IDynamoDBContext _context;
        private readonly string _tableName;
        private readonly string _indexName;

        public GetProductsByIdQuery(IDynamoDBContext context, IOptions<AwsSettings> settings)
        {
            _context = context;
            _tableName = settings.Value.DynamoDbTableName;
            _indexName = settings.Value.ProductsByIdIndexName;
        }

        public async Task<List<Product>> ExecuteAsync(string id)
        {
            var queryConfig = new QueryConfig
            {
                OverrideTableName = _tableName,
                IndexName = _indexName
            };

            var items = await _context.QueryAsync<ProductItem>(id, queryConfig).GetRemainingAsync();
            return items.Adapt<List<Product>>();
        }
    }
}
