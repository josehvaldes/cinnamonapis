namespace AWS.Cinnamon.Api.Middleware
{
    public class HeadersMiddleware
    {
        private readonly RequestDelegate _next;
        public HeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            context.Response.Headers["Permissions-Policy"] = "geolocation=()";

            context.Response.Headers["X-XSS-Protection"] = "0";
            context.Response.Headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none';";

            await _next(context);
        }
    }
}
