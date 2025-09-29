namespace DependencyInjection
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add controllers (API style for the demo).
            builder.Services.AddControllers();
            builder.Services.AddHostedService<MyHostedService>();
            builder.Services.AddSingleton<ISingletonOperation, SingletonOperation>();
            builder.Services.AddTransient<ITransientOperation, TransientOperation>();
            builder.Services.AddScoped<IScopedOperation, ScopedOperation>();
            builder.Services.AddScoped<RequestReportService>();
            builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();
            var app = builder.Build();

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}
