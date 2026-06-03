using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Cinnamon.Infrastructure.AWS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cinnamon.Seeder.AWS
{
    public class ProductSeeder
    {

        public ProductSeeder()
        {

        }

        public async Task ExecuteAsync(string filePath, string tableName) 
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("File path is not provided. Skipping product seeding.");
                return;
            }
            using (var reader = new StreamReader(filePath))
            {
                var json = await reader.ReadToEndAsync();
                var seeds = JsonConvert.DeserializeObject<List<ProductItem>>(json) ?? new List<ProductItem>();

                if (seeds.Count == 0)
                {
                    Console.WriteLine("No products found in the seed file. Skipping product seeding.");
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
                    Console.WriteLine($"Seeding {seeds.Count} products...");
                    foreach (var product in seeds)
                    {
                        await context.SaveAsync(product, queryConfig);
                    }
                    Console.WriteLine("Product seeding completed.");

                }
            }
        }
    }
}
