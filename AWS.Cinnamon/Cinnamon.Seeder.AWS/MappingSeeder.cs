using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Cinnamon.Infrastructure.AWS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Seeder.AWS
{
    public class MappingSeeder
    {
        public async Task ExecuteAsync(string filePath, string tableName) 
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("File path is not provided. Skipping mapping seeding.");
                return;
            }
            // Implement the logic to read the mapping data from the file and seed it into the database
            Console.WriteLine("Mapping seeding completed.");
            using (var reader = new StreamReader(filePath))
            {
                var json = await reader.ReadToEndAsync();
                var seeds = JsonConvert.DeserializeObject<List<MappingItem>>(json) ?? new List<MappingItem>();

                if (seeds.Count == 0)
                {
                    Console.WriteLine("No mappings found in the seed file. Skipping mapping seeding.");
                    return;
                }
                else 
                {
                    using IAmazonDynamoDB client = new AmazonDynamoDBClient();

                    var context = new DynamoDBContextBuilder()
                                    .WithDynamoDBClient(() => client)
                                    .ConfigureContext(config =>
                                    {
                                        config.DisableFetchingTableMetadata = true;
                                        config.Conversion = DynamoDBEntryConversion.V2;
                                    })
                                    .Build();

                    var queryConfig = new SaveConfig { OverrideTableName = tableName };
                    Console.WriteLine($"Seeding {seeds.Count} mappings...");
                    foreach (var mapping in seeds)
                    {
                        await context.SaveAsync(mapping, queryConfig);
                    }
                    Console.WriteLine("Mapping seeding completed.");

                }
            }

        }
    }
}
