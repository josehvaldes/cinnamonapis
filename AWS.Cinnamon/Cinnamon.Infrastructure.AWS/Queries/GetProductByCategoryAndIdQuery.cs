using Amazon.DynamoDBv2.DataModel;
using Cinnamon.Application.Interfaces.Queries;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.AWS.Model;
using Cinnamon.Infrastructure.AWS.Settings;
using Mapster;
using Microsoft.Extensions.Options;

namespace Cinnamon.Infrastructure.AWS.Queries
{
    public class GetProductByCategoryAndIdQuery : IGetProductByCategoryAndIdQuery
    {
        private readonly IDynamoDBContext _context;
        private readonly string _tableName;

        public GetProductByCategoryAndIdQuery(IDynamoDBContext context, IOptions<AwsSettings> settings)
        {
            _context = context;
            _tableName = settings.Value.DynamoDbTableName;
        }

        public async Task<Product?> ExecuteAsync(string category, string id)
        {
            var loadConfig = new LoadConfig { OverrideTableName = _tableName };

            var item = await _context.LoadAsync<ProductItem>(category, id, loadConfig);
            return item?.Adapt<Product>();
        }
    }
}
