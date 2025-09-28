namespace MiddlewarePipeline.Middlewares
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TimeProvider _timeProvider;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, TimeProvider timeProvider, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var start = _timeProvider.GetTimestamp();
            await _next(context);
            var elapsed = _timeProvider.GetElapsedTime(start);
            _logger.LogInformation("Request {Method} {Path} took {Elapsed} ms", context.Request.Method, context.Request.Path, elapsed.TotalMilliseconds);
            context.Response.Headers["X-Elapsed-Ms"] = elapsed.TotalMilliseconds.ToString("F2");
        }
    }

    public static class RequestTimingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder app)
            => app.UseMiddleware<RequestTimingMiddleware>();
    }
}