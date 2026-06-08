using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Infrastructure.AWS.Model
{
    [DynamoDBTable("ProductsNameFromSettings")]
    public class ProductItem
    {
        
        [DynamoDBHashKey]
        public string Id { get; set; } = string.Empty;

        [DynamoDBRangeKey]
        public string SortKey { get; set; } = "METADATA";


        public string Name { get; set; } = string.Empty;

        public string Img { get; set; } = string.Empty;

        public decimal Price { get; set; }
        
        public bool InStock { get; set; }
    }
}
