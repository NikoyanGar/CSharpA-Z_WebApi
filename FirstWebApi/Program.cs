namespace FirstWebApi
{
    public class Program
    {
        //entry point of the application
        public static void Main(string[] args)
        {
            // Create a new instance of WebApplication.
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Add API explorer to the container.
            builder.Services.AddEndpointsApiExplorer();

            // Add Swagger generator to the container.
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Map controllers to the request pipeline.
            app.MapControllers();

            // Run the application.
            app.Run();
        }
    }
}
