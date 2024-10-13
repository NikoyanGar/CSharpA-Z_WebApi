
namespace _001_Middleware
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

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //The IApplicationBuilder interface represents the mechanism for configuring
            //the request pipeline in an ASP.NET Core application.
            //It is used to specify how HTTP requests should be handled and processed.
            IApplicationBuilder applicationBuilder = app;

            applicationBuilder.Use(async (context, next) =>
            {
                context.Response.Headers.Append("someHeader", "someHeaderValue");
                await next();

                var myHeadher = context.Response.Headers["someHeader"];
                //if (myHeadher.Any()) change in response
                //{
                //    context.Response.StatusCode = 500;
                //}
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

