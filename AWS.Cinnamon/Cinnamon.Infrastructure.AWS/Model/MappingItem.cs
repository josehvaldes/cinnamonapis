using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Infrastructure.AWS.Model
{
    public class MappingItem
    {
        [DynamoDBHashKey]
        public string Name { get; set; } = string.Empty;
        
        [DynamoDBRangeKey]
        public string Value { get; set; } = string.Empty;
    }
}
