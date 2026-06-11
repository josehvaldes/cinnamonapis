using Asp.Versioning;
using AWS.Cinnamon.Api.Services;
using AWS.Cinnamon.Api.Settings;
using Cinnamon.Application.Handlers;
using Cinnamon.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AWS.Cinnamon.Api
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddAPIDependencies(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ILinkService, LinkService>();

            services.AddVersioningConfig(config);
            services.AddCorsConfig(config);
            services.AddRateLimitingConfig(config);

            services.AddCustomHealthChecks(config);
            services.AddCacheProfiles(config);

            

            return services;
        }

        public static IServiceCollection AddCustomHealthChecks(
            this IServiceCollection services, IConfiguration config)
        {

            services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });
            return services;
        }
        public static IServiceCollection AddRateLimitingConfig(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("fixed", o =>
                {
                    o.PermitLimit = 50;
                    o.Window = TimeSpan.FromMinutes(1);
                });
            });

            return services;
        }

        public static IServiceCollection AddCorsConfig(
            this IServiceCollection services, IConfiguration config)
        {

            var corsSettings = config.GetSection("Cors")
                             .Get<CorsSettings>() ?? new CorsSettings();

            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCors", policy =>
                {
                    policy
                        .WithOrigins(corsSettings.AllowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

                options.AddPolicy("AllowCloudflare", policy =>
                {
                    policy
                        .WithOrigins(corsSettings.AllowedOrigins)
                        .WithHeaders("x-api-key", "Content-Type")
                        .WithMethods("GET", "OPTIONS");
                });

                // Strict policy for sensitive endpoints
                options.AddPolicy("StrictCors", policy =>
                {
                    policy
                        .WithOrigins(corsSettings.AllowedOrigins)
                        .WithHeaders("Content-Type", "Authorization")
                        .WithMethods("GET", "POST", "PUT", "DELETE");
                });

            });
            return services;
        }

        public static IServiceCollection AddVersioningConfig(
            this IServiceCollection services, IConfiguration config)
        {

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),   // /api/v1/products
                    new HeaderApiVersionReader("X-Api-Version")); // optional fallback
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            return services;
        }

        public static IServiceCollection AddCacheProfiles(
            this IServiceCollection services, IConfiguration config)
        {
            services.AddControllersWithViews(options => { 
                options.CacheProfiles.Add("Public5min", new CacheProfile
                {
                    Duration = 300,
                    Location = ResponseCacheLocation.Any,
                    NoStore = false
                });
            });

            return services;
        }

    }
}
