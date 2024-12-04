using Microsoft.EntityFrameworkCore;
using RedisExplain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AppDbContext>(opt => { opt.UseInMemoryDatabase(databaseName: "demoDb"); });
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("RedisConfiguration:Host").Value;
});
//Another Redis Desktop Manager for Redis UI
//docker run -d --name redis-stack-server -p 6379:6379 redis / redis - stack - server:latest
//docker run -d --name redis-server -p 6379:6379 redis

builder.Services.AddScoped<ICacheService, DistributedCacheService>();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
