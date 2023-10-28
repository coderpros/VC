namespace VC.Res.WebInterface.Middleware
{
    public class APIKeyMiddleware
    {
        private readonly RequestDelegate _next;

        public APIKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-API-Key", out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided.");

                return;
            }

            if (!Core.Settings.Interface.Fetch.APIKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client.");

                return;
            }

            await _next(context);
        }
    }
}
