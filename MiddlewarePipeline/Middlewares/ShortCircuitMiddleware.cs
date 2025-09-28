namespace MiddlewarePipeline.Middlewares
{
    public class ShortCircuitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ShortCircuitMiddleware> _logger;

        public ShortCircuitMiddleware(RequestDelegate next, ILogger<ShortCircuitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/forbidden"))
            {
                _logger.LogWarning("Short-circuiting request to {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access forbidden (short-circuited middleware)\n");
                return; // pipeline stops here
            }

            await _next(context);
        }
    }
}