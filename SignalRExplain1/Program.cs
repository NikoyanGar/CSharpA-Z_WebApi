
using SignalRExplain1.Hubs;

namespace SignalRExplain1
{
    //If you need to introduce real-time functionality to your application in .NET, there's one library you will most likely reach for - SignalR
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

            // Add SignalR services to the container.
            builder.Services.AddSignalR();

            var app = builder.Build();

            // Map the SignalR hub to the specified path.
            app.MapHub<NotificationsHub>("notifications-hub");
            //wss://{applicationUrl}/notifications-hub

            // {
            //  "protocol": "json",
            //  "version": 1
            //}?

            //{
            // "arguments": ["This is the notification message."],
            //"target": "SendNotification",
            //"type": 1
            //}?

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
