using Amazon.DynamoDBv2.DataModel;
using Cinnamon.Application.Interfaces.Queries;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.AWS.Model;
using Cinnamon.Infrastructure.AWS.Settings;
using Mapster;
using Microsoft.Extensions.Options;

namespace Cinnamon.Infrastructure.AWS.Queries
{
    public class GetOnSalesQuery : IGetOnSalesQuery
    {
        private readonly IDynamoDBContext _context;
        private readonly string _tableName;

        public GetOnSalesQuery(IDynamoDBContext context, IOptions<AwsSettings> settings)
        {
            _context = context;
            _tableName = settings.Value.DynamoDbTableName;
        }

        public async Task<List<Product>> ExecuteAsync()
        {
            var queryConfig = new QueryConfig { OverrideTableName = _tableName };

            var items = await _context.QueryAsync<ProductItem>("on-sales", queryConfig).GetRemainingAsync();
            return items.Adapt<List<Product>>();
        }
    }
}
