
namespace _001_WebApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            // Accessing the functionality of the application
            var configuration = app.Configuration;
            var environment = app.Environment;
            var lifetime = app.Lifetime;
            var logger = app.Logger;
            var services = app.Services;


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            //app.Run();

            await app.StartAsync();
            await Task.Delay(10000);
            await app.StopAsync();

        }
    }
}
