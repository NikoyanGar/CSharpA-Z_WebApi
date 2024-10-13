namespace _002_Middleware
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<LoggingMiddleware>(); // Add LoggingMiddleware to the pipeline
            app.UseMiddleware<AuthenticationMiddleware>(); // Add AuthenticationMiddleware to the pipeline

            app.MapControllers();

            app.Run();
        }
    }

    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log the incoming request
            Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");

            // Call the next middleware in the pipeline
            await _next(context);

            // Log the outgoing response
            Console.WriteLine($"Outgoing Response: {context.Response.StatusCode}");
        }
    }

    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the Authorization header exists
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                // Perform authentication logic here
                // ...
            }
            else
            {
                // Handle unauthorized access
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
