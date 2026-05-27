namespace AWS.Cinnamon.Api.Settings
{
    public sealed class CorsSettings
    {
        public string[] AllowedOrigins { get; init; } = Array.Empty<string>();
    }
}
