namespace MiddlewarePipeline.Middlewares
{
    public class HeaderMutationMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderMutationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add a request-scoped item
            context.Items["StartTimeUtc"] = DateTime.UtcNow;

            // Add/modify headers
            context.Request.Headers.TryAdd("X-Demo-Injected", "true");
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Pipeline-Stage"] = "HeaderMutationCompleted";
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}