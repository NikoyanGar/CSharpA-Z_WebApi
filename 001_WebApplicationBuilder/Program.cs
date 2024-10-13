using Newtonsoft.Json;

namespace _001_WebApplicationBuilder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            Console.WriteLine(" start builder.Services-------------------------------------------------");
            foreach (var service in builder.Services)
            {
                Console.WriteLine($"Service Type: {service.ServiceType}");
            }
            Console.WriteLine("start builder.Host.Properties ------------------------------------------------");
            foreach (var keyValuePair in builder.Host.Properties)
            {
                Console.WriteLine($"Key: {keyValuePair.Key}, Value: {keyValuePair.Value}");
            }

            Console.WriteLine("start builder.Environment-----------------------------------------------");
            Console.WriteLine($"Environment Name: {builder.Environment.EnvironmentName}");
            Console.WriteLine($"Application Name: {builder.Environment.ApplicationName}");
            Console.WriteLine($"Content Root File Provider: {builder.Environment.ContentRootFileProvider}");
            Console.WriteLine($"Content Root Path: {builder.Environment.ContentRootPath}");
            Console.WriteLine($"Web Root Path: {builder.Environment.WebRootPath}");

            Console.WriteLine("start  builder.Configuration-----------------------------------------------");

            foreach (var keyValuePair in builder.Configuration.AsEnumerable())
            {
                Console.WriteLine($"Key: {keyValuePair.Key}, Value: {keyValuePair.Value}");
            }

            Console.WriteLine("start builder.WebHost-----------------------------------------------");

            var builderJson = JsonConvert.SerializeObject(builder.WebHost);
            Console.WriteLine($"Builder JSON: {builderJson}");
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
