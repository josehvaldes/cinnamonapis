using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Cinnamon.Application.Interfaces.Queries;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.AWS.Model;
using Cinnamon.Infrastructure.AWS.Settings;
using Mapster;
using Microsoft.Extensions.Options;

namespace Cinnamon.Infrastructure.AWS.Queries
{
    public class GetProductsByCategoryAndInStock : IGetProductsByCategoryAndInStock
    {
        private readonly IDynamoDBContext _context;
        private readonly string _tableName;

        public GetProductsByCategoryAndInStock(IDynamoDBContext context, IOptions<AwsSettings> settings)
        {
            _context = context;
            _tableName = settings.Value.DynamoDbTableName;
        }

        public async Task<List<Product>> ExecuteAsync(string category, bool inStock)
        {
            var queryConfig = new QueryConfig
            {
                OverrideTableName = _tableName,
                QueryFilter = [new ScanCondition(nameof(ProductItem.InStock), ScanOperator.Equal, inStock)]
            };

            var items = await _context.QueryAsync<ProductItem>(category, queryConfig)
                                      .GetRemainingAsync();
            return items.Adapt<List<Product>>();
        }
    }
}
