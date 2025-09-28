using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace StartupExplained
{

    public class Program
    {
        // In .NET 6+ minimal hosting merges:
        // - CreateHostBuilder / Startup.ConfigureServices -> now the builder.Services section
        // - Startup.Configure (middleware pipeline) -> now the app.* section
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ----------------------------------------------------
            // CONFIGURATION (builder.Configuration)
            // Additional configuration sources can be added here.
            // builder.Configuration.AddJsonFile("extra.json", optional: true);
            // ----------------------------------------------------

            // ----------------------------------------------------
            // LOGGING (builder.Logging)
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            // ----------------------------------------------------

            // ----------------------------------------------------
            // SERVICES (formerly Startup.ConfigureServices)
            // Framework services
            builder.Services.AddControllers();          // API controllers
            builder.Services.AddRazorPages();           // Razor Pages (project note)
            builder.Services.AddEndpointsApiExplorer(); // For minimal endpoints description
            //builder.Services.AddSwaggerGen();           // OpenAPI / Swagger
            builder.Services.AddResponseCaching();
            builder.Services.AddMemoryCache();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddHealthChecks();

            // Options pattern binding
            builder.Services.Configure<MyFeatureOptions>(
                builder.Configuration.GetSection(MyFeatureOptions.SectionName));

            // Typed options validation example
            builder.Services.AddOptions<MyFeatureOptions>()
                .PostConfigure(o =>
                {
                    if (o.RefreshSeconds <= 0) o.RefreshSeconds = 30;
                })
                .Validate(o => o.RefreshSeconds >= 5, "RefreshSeconds must be >= 5");

            // CORS example
            builder.Services.AddCors(o =>
            {
                o.AddPolicy("DefaultCors", p =>
                    p.WithOrigins("https://localhost:5001")
                     .AllowAnyHeader()
                     .AllowAnyMethod());
            });

            // DI lifetimes
            builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>(); // Singleton
            builder.Services.AddScoped<IRequestIdProvider, RequestIdProvider>(); // Scoped
            builder.Services.AddTransient<GuidFactory>(); // Transient

            // HttpClient factory
            builder.Services.AddHttpClient("github", c =>
            {
                c.BaseAddress = new Uri("https://api.github.com/");
                c.DefaultRequestHeaders.UserAgent.ParseAdd("StartupExplainedSample");
            });

            // Fine-tune JSON (global)
            builder.Services.Configure<JsonOptions>(o =>
            {
                o.SerializerOptions.PropertyNamingPolicy = null;
                o.SerializerOptions.WriteIndented = true;
            });

            var app = builder.Build();

            // ----------------------------------------------------
            // MIDDLEWARE PIPELINE (formerly Startup.Configure)
            // ORDER MATTERS!
            // ----------------------------------------------------

            // Error handling (environment-specific)
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI();
            }
            else
            {
                // Global exception handler -> RFC 7807 style response
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        var feature = context.Features.Get<IExceptionHandlerFeature>();
                        var ex = feature?.Error;
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        var problem = Results.Problem(
                            detail: ex?.Message,
                            title: "An unexpected error occurred",
                            statusCode: StatusCodes.Status500InternalServerError);
                        await problem.ExecuteAsync(context);
                    });
                });
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Static files (wwwroot) - place before routing if added
            // app.UseStaticFiles();

            app.UseCors("DefaultCors");

            app.UseRouting(); // Must appear before auth/authorization and endpoints

            // Authentication (if added)
            // app.UseAuthentication();
            app.UseAuthorization();

            app.UseResponseCaching();

            // Custom inline middleware (example)
            app.Use(async (context, next) =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                await next();
                sw.Stop();
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Request {Path} completed in {Elapsed}ms",
                    context.Request.Path, sw.ElapsedMilliseconds);
            });

            // Map Health Checks
            app.MapHealthChecks("/health");

            // Minimal API endpoint (DI demo)
            app.MapGet("/time", (ITimeProvider clock, IRequestIdProvider req, IOptions<MyFeatureOptions> opts) =>
            {
                return Results.Ok(new
                {
                    Utc = clock.UtcNow,
                    RequestId = req.Id,
                    FeatureEnabled = opts.Value.Enabled
                });
            });

            // HttpClient factory usage endpoint
            app.MapGet("/github-rate", async (IHttpClientFactory factory) =>
            {
                var client = factory.CreateClient("github");
                using var resp = await client.GetAsync("rate_limit");
                var json = await resp.Content.ReadAsStringAsync();
                return Results.Content(json, MediaTypeNames.Application.Json);
            });

            // Razor Pages & Controllers endpoints
            app.MapRazorPages();
            app.MapControllers();

            // Fallback minimal endpoint
            app.MapGet("/", (ITimeProvider clock, IOptions<MyFeatureOptions> opts) =>
                $"Welcome. UtcNow={clock.UtcNow:O} FeatureEnabled={opts.Value.Enabled}");

            app.Run();
        }
    }
}

/*
SUMMARY CHEAT SHEET

ConfigureServices equivalent:
- builder.Services.AddXyz();
- Add DI registrations, options, logging, EF Core, auth, caching, HttpClient.

Configure equivalent (pipeline):
- Error handling / HSTS
- HTTPS redirection
- Static files (optional)
- CORS
- Routing
- Authentication
- Authorization
- ResponseCaching / other infra
- Custom middleware (Use / Map)
- Endpoint mapping (MapControllers, MapRazorPages, MapGet, MapGroup, etc.)
*/
