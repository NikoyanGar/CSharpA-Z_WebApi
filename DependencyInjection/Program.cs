namespace DependencyInjection
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add controllers (API style for the demo).
            builder.Services.AddControllers();

            // DI registrations demonstrating lifetimes.
            builder.Services.AddTransient<ITransientOperation, TransientOperation>();
            builder.Services.AddScoped<IScopedOperation, ScopedOperation>();
            builder.Services.AddSingleton<ISingletonOperation, SingletonOperation>();

            // Additional singleton used by scoped service.
            builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();

            // Scoped aggregator that consumes all lifetimes (and two transients).
            builder.Services.AddScoped<RequestReportService>();

            // Singleton that needs a scoped service - uses IServiceScopeFactory properly.
            builder.Services.AddSingleton<ISingletonConsumer, SingletonConsumer>();

            // (Anti-pattern) Captive dependency example:
            // DO NOT REGISTER this - uncommenting would cause the scoped dependency to act like a singleton.
            // builder.Services.AddSingleton<BadSingletonCapturingScoped>();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}
