namespace MiddlewarePipeline.Middlewares
{
    public class CounterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CounterState _state;
        private readonly ILogger<CounterMiddleware> _logger;

        public CounterMiddleware(RequestDelegate next, CounterState state, ILogger<CounterMiddleware> logger)
        {
            _next = next;
            _state = state;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _state.Increment();
            _logger.LogInformation("Request count = {Count}", _state.RequestCount);
            context.Response.Headers["X-Request-Count"] = _state.RequestCount.ToString();
            await _next(context);
        }
    }
}