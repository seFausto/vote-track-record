using Microsoft.Extensions.Options;
using VotingTrackRecord.Common.Settings;

namespace VotingTrackRecord.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly ApplicationSettings ApplicationSettings;
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next,
            IOptions<ApplicationSettings> applicationSettings)
        {
            this.ApplicationSettings = applicationSettings.Value;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(nameof(ApplicationSettings.XApiKey)
                , out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided ");
                return;
            }

            var apiKey = this.ApplicationSettings.XApiKey;
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }
            await _next(context);
        }
    }
}
