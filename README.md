# CinnamonAPIs

Backend API for the **CinnamonPages** e-commerce storefront. Built as a portfolio project to demonstrate backend skills using **ASP.NET Core (.NET 10)** deployed as an **AWS Lambda** function behind **AWS API Gateway** via the AWS Serverless Application Model (SAM).

The companion frontend application lives at [CinnamonPages](https://github.com/josehvaldes/CinnamonPages) and is served through Cloudflare.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 (ASP.NET Core) |
| Hosting | AWS Lambda (via `Amazon.Lambda.AspNetCoreServer`) |
| API Gateway | AWS API Gateway REST API |
| IaC / Deployment | AWS SAM (`serverless.template`) |
| Object Mapping | Mapster |
| Data Store | AWS DynamoDB (via `AWSSDK.DynamoDBv2`) |
| API Versioning | `Asp.Versioning` |
| Region | `us-east-2` |

---

## Solution Structure

```
cinnamonapis/
├── AWS.Cinnamon.Api              # Entry point – ASP.NET Core app wired to Lambda
├── Cinnamon.Application          # Application layer (handlers, query interfaces)
├── Cinnamon.Contracts            # Request/response DTOs shared across layers
├── Cinnamon.Domain               # Core domain entities (e.g. Product)
├── Cinnamon.Infrastructure.AWS   # DynamoDB implementations of query interfaces
└── Cinnamon.Seeder.AWS           # CLI tool to seed DynamoDB with product data
```

### Project Responsibilities

- **AWS.Cinnamon.Api** — Configures the ASP.NET Core pipeline, registers dependencies, and exposes REST controllers. Uses `AddAWSLambdaHosting(LambdaEventSource.RestApi)` so the same codebase runs locally via Kestrel and in Lambda without changes.
- **Cinnamon.Application** — Contains the `IHandler` interface, `ProductHandler` implementation, and the query interfaces (`IGetProductsByIdQuery`, `IGetProductsByCategoryQuery`, `IGetProductsByCategoryAndInStock`).
- **Cinnamon.Contracts** — Defines response models (`HomepageResponse`, `ProductResponse`) decoupled from domain entities.
- **Cinnamon.Domain** — Plain domain entities (`Product`) with no external dependencies.
- **Cinnamon.Infrastructure.AWS** — Implements all query interfaces against **AWS DynamoDB** using `AWSSDK.DynamoDBv2`. Registers services via `AddAWSDependencies(IConfiguration)` extension method. `AwsSettings` (bound from the `AWS` config section) provides both table names and the GSI index name. DynamoDB models are `ProductItem` (products table) and `MappingItem` (mappings table).
- **Cinnamon.Seeder.AWS** — .NET console app (Docker-ready) that reads `Data/products.json` and seeds the DynamoDB products table. Reads configuration from `appsettings.json` / environment variables.

---

## API Endpoints

### Homepage — `GET /api/v1/homepage`

Returns curated product lists for the storefront homepage.

**Response shape:**

```json
{
  "trendingProducts": [{ "name": "Cinnamon Roll", "img": "/products/...", "price": 2.99 }],
  "newArrivals":      [{ "name": "Blueberry Muffin", "img": "/products/...", "price": 2.49 }],
  "onSales":          [{ "name": "Apple Pie", "img": "/products/...", "price": 4.99 }]
}
```

Response is cached for **5 minutes** (`Cache-Control: public, max-age=300`).

---

### Get Product — `GET /api/v1/products/{id}`

Returns a single product by its ID, including HATEOAS hypermedia links.

**Response shape:**

```json
{
  "id": "prod-001",
  "name": "Cinnamon Roll",
  "img": "/products/cinnamon-roll.jpg",
  "price": "2.99",
  "links": [
    { "href": "/api/v1/products/prod-001", "rel": "self", "method": "GET" }
  ]
}
```

Response is cached for **5 minutes** (`Cache-Control: public, max-age=300`).

---

### Get Products by Category — `GET /api/v1/products/category/{category}`

Returns a paginated list of products belonging to the given category, each with HATEOAS links.

**Query parameters:**

| Parameter | Type | Default | Constraints |
|---|---|---|---|
| `pageNumber` | `int` | `1` | ≥ 1 |
| `pageSize` | `int` | `20` | 1 – 100 |

**Response shape:**

```json
{
  "items": [
    {
      "id": "prod-001",
      "name": "Cinnamon Roll",
      "img": "/products/cinnamon-roll.jpg",
      "price": "2.99",
      "links": [
        { "href": "/api/v1/products/prod-001", "rel": "self", "method": "GET" }
      ]
    }
  ],
  "totalCount": 42,
  "pageNumber": 1,
  "pageSize": 20,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

Response is cached for **5 minutes** (`Cache-Control: public, max-age=300`).

---

## Key Features

- **API Versioning** — URL segment (`/api/v1/`) and `X-Api-Version` header are both supported.
- **Rate Limiting** — Fixed-window limiter: 50 requests per minute per client.
- **CORS** — `AllowCloudflare` policy restricts origins to the configured frontend domain; locally defaults to `http://localhost:5173`.
- **Global Exception Handling** — Centralized `GlobalExceptionHandler` returns RFC 7807 Problem Details on errors.
- **Response Caching** — `Public5min` cache profile applied to all endpoints.
- **HATEOAS Links** — `ProductResponse` includes hypermedia `links` (href / rel / method) generated by `ILinkService`.
- **Pagination** — `PagedResponse<T>` returned by category queries; controlled via `pageNumber` and `pageSize` query parameters.
- **Health Check** — Liveness probe available for monitoring.

---

## AWS Infrastructure

Defined in `serverless.template` (AWS SAM):

| Resource | Value |
|---|---|
| Lambda handler | `AWS.Cinnamon.Api` |
| Runtime | `dotnet10` |
| Memory | 256 MB |
| Timeout | 15 s |
| IAM Policy | `AWSLambdaBasicExecutionRole` |
| S3 bucket (artifacts) | `cinnamonapi-cloudformation` |
| CloudFormation stack | `cinnamonapiv1` |
| API Gateway stage URL | `https://<id>.execute-api.us-east-2.amazonaws.com/Prod/` |

---

## Local Development

**Prerequisites:** .NET 10 SDK, AWS CLI (optional for deployment)

```bash
# Restore & run locally
cd aws.cinnamon/AWS.Cinnamon.Api
dotnet run
# API available at https://localhost:<port>/api/v1/homepage
```

### Seeding DynamoDB

`Cinnamon.Seeder.AWS` is a standalone console app that populates the DynamoDB table from `Data/products.json`.

```bash
cd aws.cinnamon/Cinnamon.Seeder.AWS
dotnet run
```

Configure the target table via `appsettings.json` or the `AWS__DynamoDbTableName` environment variable. A `Dockerfile` is included for running the seeder as a container.

### Configuration (`AwsSettings`)

The infrastructure layer reads the following keys from the `AWS` configuration section:

| Key | Description |
|---|---|
| `AccessKey` | AWS access key (not needed when using IAM roles) |
| `SecretKey` | AWS secret key (not needed when using IAM roles) |
| `Region` | AWS region (e.g. `us-east-2`) |
| `ProductsTableName` | Name of the DynamoDB products table |
| `MappingsTableName` | Name of the DynamoDB mappings table (category → product ID lookups) |
| `ProductsByIdIndexName` | GSI name used for product-by-id lookups |

---

## Deployment

Uses the [AWS Lambda Tools for .NET](https://github.com/aws/aws-extensions-for-dotnet-cli):

```bash
dotnet tool install -g Amazon.Lambda.Tools   # first time only

cd aws.cinnamon/AWS.Cinnamon.Api
dotnet lambda deploy-serverless \
  --stack-name cinnamonapiv1 \
  --s3-bucket cinnamonapi-cloudformation \
  --region us-east-2
```

Default values are stored in `aws-lambda-tools-defaults.json` and picked up automatically by Visual Studio's AWS Toolkit publish wizard.

---

## Related Repositories

- **Frontend:** [CinnamonPages](https://github.com/josehvaldes/CinnamonPages) — storefront UI consuming this API

## AWS S3
- a private bucket to contain product images.

## AWS DynamoDB

The data model uses **two separate DynamoDB tables**:

### Products Table (`ProductsTableName`)

Stores the full product records. Uses a composite key:

| Attribute | Role | Value |
|---|---|---|
| `Id` | Hash key (partition key) | Product ID (e.g. `prod-001`) |
| `SortKey` | Range key | Fixed value `"METADATA"` |

Additional attributes: `Name`, `Img`, `Price` (decimal), `InStock` (bool).

Mapped to the `ProductItem` DynamoDB model in `Cinnamon.Infrastructure.AWS`.

### Mappings Table (`MappingsTableName`)

Stores relationship data used to resolve queries such as *"all products in a category"* or *"trending / new arrivals / on-sale"* lists. Uses a composite key:

| Attribute | Role | Value example |
|---|---|---|
| `Name` | Hash key (partition key) | `CATEGORY#bakery`, `CATEGORY#trending` |
| `Value` | Range key | Product ID that belongs to that group |

Mapped to the `MappingItem` DynamoDB model in `Cinnamon.Infrastructure.AWS`.

The `GetProductsByCategoryQuery` first queries the mappings table to resolve product IDs, then performs a batch-get against the products table.

## AWS API GATEWAY

Current features implemented in AWS API GATEWAY:
- Custom domain name connected to cloudflare
- x-api-key
- Usage plan protection 
   * rage 5 request per second
   * Burst 10 request
   * Quota 700 requests per day.
   
## AWS CloudFront
- Configured a stardard Distribution with a alternative domain name from cloudflare to serve S3 images.