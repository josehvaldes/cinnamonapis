using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace AWS.Cinnamon.Api
{
    public static class ApplicationMapping
    {
        public static void MapCustomBehaviors(this WebApplication app) 
        {
            //Map HealthCheck to root for easy access
            app.MapHealthChecks("/api/health").AllowAnonymous();

            app.MapHealthChecks("/api/health/live", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live")
            });
        }
    }
}
