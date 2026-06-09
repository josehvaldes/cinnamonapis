using AWS.Cinnamon.Api;
using AWS.Cinnamon.Api.Mapping;
using AWS.Cinnamon.Api.Middleware;
using Cinnamon.Infrastructure.AWS;

MappingConfig.RegisterMappings();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddResponseCaching();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
builder.Services.AddAPIDependencies(builder.Configuration);
builder.Services.AddAWSDependencies(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("AllowCloudflare");
app.UseRateLimiter();
app.MapCustomBehaviors(); // Extension method for any custom middleware or behaviors
app.UseMiddleware<HeadersMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Welcome to running ASP.NET Core Minimal API on AWS Lambda");

app.Run();
