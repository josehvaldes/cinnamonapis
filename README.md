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
| API Versioning | `Asp.Versioning` |
| Region | `us-east-2` |

---

## Solution Structure

```
cinnamonapis/
├── AWS.Cinnamon.Api          # Entry point – ASP.NET Core app wired to Lambda
├── Cinnamon.Application      # Application layer (handlers, interfaces)
├── Cinnamon.Contracts        # Request/response DTOs shared across layers
└── Cinnamon.Domain           # Core domain entities (e.g. Product)
```

### Project Responsibilities

- **AWS.Cinnamon.Api** — Configures the ASP.NET Core pipeline, registers dependencies, and exposes REST controllers. Uses `AddAWSLambdaHosting(LambdaEventSource.RestApi)` so the same codebase runs locally via Kestrel and in Lambda without changes.
- **Cinnamon.Application** — Contains the `IHandler` interface and `ProductHandler` implementation that provides product data (trending, new arrivals, on-sale).
- **Cinnamon.Contracts** — Defines response models (`HomepageResponse`, `ProductResponse`) decoupled from domain entities.
- **Cinnamon.Domain** — Plain domain entities (`Product`) with no external dependencies.

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

## Key Features

- **API Versioning** — URL segment (`/api/v1/`) and `X-Api-Version` header are both supported.
- **Rate Limiting** — Fixed-window limiter: 50 requests per minute per client.
- **CORS** — `AllowCloudflare` policy restricts origins to the configured frontend domain; locally defaults to `http://localhost:5173`.
- **Global Exception Handling** — Centralized `GlobalExceptionHandler` returns RFC 7807 Problem Details on errors.
- **Response Caching** — `Public5min` cache profile applied to homepage endpoint.
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
