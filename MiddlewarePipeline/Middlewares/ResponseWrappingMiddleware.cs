namespace MiddlewarePipeline.Middlewares
{
    public class ResponseWrappingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseWrappingMiddleware> _logger;

        public ResponseWrappingMiddleware(RequestDelegate next, ILogger<ResponseWrappingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only wrap for text responses
            var originalBody = context.Response.Body;
            using var buffer = new MemoryStream();
            context.Response.Body = buffer;

            await _next(context);

            // After downstream
            if (context.Response.ContentType != null &&
                context.Response.ContentType.Contains("text", StringComparison.OrdinalIgnoreCase))
            {
                buffer.Seek(0, SeekOrigin.Begin);
                var originalText = await new StreamReader(buffer).ReadToEndAsync();
                buffer.SetLength(0);

                var wrapperHeader = $"--- Begin Wrapped ({DateTime.UtcNow:O}) ---\n";
                var wrapperFooter = $"\n--- End Wrapped Status={context.Response.StatusCode} ---\n";
                var final = wrapperHeader + originalText + wrapperFooter;

                var bytes = System.Text.Encoding.UTF8.GetBytes(final);
                await originalBody.WriteAsync(bytes);
            }
            else
            {
                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(originalBody);
            }

            context.Response.Body = originalBody;
            _logger.LogDebug("Response wrapped (Path: {Path})", context.Request.Path);
        }
    }
}