using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Infrastructure.AWS.Settings
{
    public class AwsSettings
    {
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string ProductsTableName { get; set; } = string.Empty;
        public string MappingsTableName { get; set; } = string.Empty;
        public string SQSQueueUrl { get; set; } = string.Empty;
    }
}
