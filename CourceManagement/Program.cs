using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CourceManagement.Data.Entities;
using CourceManagement.Data;
using CourceManagement.Services;
using CourceManagement.ViewModels;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CourseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CourseDbContext") ?? throw new InvalidOperationException("Connection string 'CourseDbContext' not found.")));

//Setup identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<CourseDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

//DI configuration
builder.Services.AddTransient<ICoursesService, CoursesService>();
builder.Services.AddTransient<ILessonsService, LessonsService>();
builder.Services.AddTransient<IUsersService, UsersService>();

//File Storage
builder.Services.AddTransient<IStorageService, FileStorageService>();

//automapper configuration
builder.Services.AddAutoMapper(typeof(Program));

//DbInitializer
builder.Services.AddTransient<DbInitializer>();

//Jwt Authentication
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger Course Management", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Course Management V1");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    //docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P@ssw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest

    var db = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
    db.Database.Migrate();
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Seeding data...");
        var dbInitializer = serviceProvider.GetService<DbInitializer>();
        if (dbInitializer != null)
            dbInitializer.Seed()
                         .Wait();
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();