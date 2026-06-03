using Cinnamon.Seeder.AWS;
using Microsoft.Extensions.Configuration;
Console.WriteLine("Cinnamon Products Seeder");
Console.WriteLine($"Env: {Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}");


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();

var dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
var tablename = configuration.GetValue<string>("AWS:DynamoDbTableName") ?? "ProductsTable";
var seeders = new Dictionary<string, Func<Task>>(StringComparer.OrdinalIgnoreCase) {
    [nameof(ProductSeeder)] = () => new ProductSeeder().ExecuteAsync(Path.Combine(dataDir, "products.json"), tablename),
};

Console.WriteLine("No command provided. Seeding all by default...");
await Task.WhenAll(seeders.Values.Select(seed => seed()));

Console.WriteLine("Seeding completed.");
