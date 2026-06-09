using Amazon.DynamoDBv2.DataModel;
using Cinnamon.Application.Common;
using Cinnamon.Application.Interfaces.Queries;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.AWS.Model;
using Cinnamon.Infrastructure.AWS.Settings;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cinnamon.Infrastructure.AWS.Queries
{
    public class GetProductsByCategoryQuery : IGetProductsByCategoryQuery
    {
        private readonly IAsyncPolicy _policy;
        private readonly IDynamoDBContext _context;
        private readonly string _productsTableName;
        private readonly string _mappingsTableName;
        private readonly ILogger<GetProductsByCategoryQuery> _logger;

        public GetProductsByCategoryQuery(IDynamoDBContext context, IOptions<AwsSettings> settings, IAsyncPolicy policy, ILogger<GetProductsByCategoryQuery> logger)
        {
            _context = context;
            _productsTableName = settings.Value.ProductsTableName;
            _mappingsTableName = settings.Value.MappingsTableName;
            _policy = policy;
            _logger = logger;
        }

        public async Task<List<Product>> ExecuteAsync(string category)
        {
            var sw = Stopwatch.StartNew();

            var mappingQueryConfig = new QueryConfig { OverrideTableName = _mappingsTableName };
            var mappings = await _policy.ExecuteAsync(async () =>
            {
                var items = await _context.QueryAsync<MappingItem>($"CATEGORY#{category}", mappingQueryConfig).GetRemainingAsync();
                return items;
            });
            sw.Stop();
            _logger.LogInformation($"GetProductsByCategoryQuery.ExecuteAsync({category}) - Mappings query took {sw.ElapsedMilliseconds} ms.");

            sw.Start();
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

            var products = await _policy.ExecuteAsync(async () =>
            {
                await getbatch.ExecuteAsync();
                return getbatch.Results.Adapt<List<Product>>();
            });
            sw.Stop();
            _logger.LogInformation($"GetProductsByCategoryQuery.ExecuteAsync({category}) - {products.Count} Products batch query took {sw.ElapsedMilliseconds} ms.");
            return products;
        }

        public async Task<PagedResult<Product>> ExecuteAsync(string category, int pageNumber, int pageSize)
        {
            var sw = Stopwatch.StartNew();
            var mappingQueryConfig = new QueryConfig { OverrideTableName = _mappingsTableName };
            var mappings = await _policy.ExecuteAsync(async () =>
            {
                var items = await _context.QueryAsync<MappingItem>($"CATEGORY#{category}", mappingQueryConfig).GetRemainingAsync();
                return items;
            });
            sw.Stop();
            _logger.LogInformation($"GetProductsByCategoryQuery.ExecuteAsync({category}) - Mappings query took {sw.ElapsedMilliseconds} ms.");

            if (mappings == null || !mappings.Any())
            {
                return new PagedResult<Product>([], 0, pageNumber, pageSize);
            }

            var totalCount = mappings.Count;

            var pagedMappings = mappings
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (!pagedMappings.Any())
            {
                return new PagedResult<Product>([], totalCount, pageNumber, pageSize);
            }

            sw.Start();
            var queryConfig = new BatchGetConfig { OverrideTableName = _productsTableName };
            var getbatch = _context.CreateBatchGet<ProductItem>(queryConfig);
            foreach (var mapping in pagedMappings)
            {
                getbatch.AddKey(mapping.Value, "METADATA");
            }
            
            var products = await _policy.ExecuteAsync(async () =>
            {
                await getbatch.ExecuteAsync();
                return getbatch.Results.Adapt<List<Product>>();
            });
            sw.Stop();
            _logger.LogInformation($"GetProductsByCategoryQuery.ExecuteAsync({category}) - {products.Count} Products batch query took {sw.ElapsedMilliseconds} ms.");
            return new PagedResult<Product>(products, totalCount, pageNumber, pageSize);
        }
    }
}
