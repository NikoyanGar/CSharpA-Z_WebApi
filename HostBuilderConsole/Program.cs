using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;


namespace HostBuilderConsole
{
    internal class Program
    {
        // Run with: dotnet run            -> Generic Host example
        //           dotnet run -- --webhost -> Legacy Web Host example
        static async Task Main(string[] args)
        {
            if (args.Contains("--webhost", StringComparer.OrdinalIgnoreCase))
            {
                await RunLegacyWebHostAsync(args);
            }
            else
            {
                await RunGenericHostAsync(args);
            }
        }

        // 1) MODERN / RECOMMENDED: Generic Host
        // Host.CreateDefaultBuilder:
        // - Loads configuration (appsettings.json, env vars, command line, user secrets in dev)
        // - Sets up logging (Console, Debug, EventSource)
        // - Registers default services & DI container
        private static async Task RunGenericHostAsync(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg =>
                {
                    // You can add/override config sources here
                    // cfg.AddJsonFile("extra.json", optional: true);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<TimeReporterBackgroundService>(); // Background worker
                    services.AddSingleton<ISystemClock, SystemClock>();
                })
                // Add web server pipeline (Kestrel) via the generic host
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Configure(app =>
                    {
                        // Minimal request delegate (no controllers / Razor here – just to illustrate)
                        app.Run(async context =>
                        {
                            var clock = context.RequestServices.GetRequiredService<ISystemClock>();
                            await context.Response.WriteAsync($"Legacy WebHost response. UtcNow: {clock.UtcNow:O}");
                        });
                    });
                })
                .Build();

            await host.RunAsync();
        }

        // 2) LEGACY (older style): Web Host
        // WebHost.CreateDefaultBuilder:
        // - Pre-dates the Generic Host
        // - Focused only on web workloads (no built-in support path for non-HTTP background scenarios without extra plumbing)
        private static async Task RunLegacyWebHostAsync(string[] args)
        {
            using var webHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureAppConfiguration((ctx, cfg) =>
                {
                    cfg.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                       .AddEnvironmentVariables();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ISystemClock, SystemClock>();
                })
                .Configure(app =>
                {
                    app.Run(async context =>
                    {
                        var clock = context.RequestServices.GetRequiredService<ISystemClock>();
                        await context.Response.WriteAsync($"Legacy WebHost response. UtcNow: {clock.UtcNow:O}");
                    });
                })
                .Build();

            await webHost.RunAsync();
        }
    }

    // Simple service to demonstrate DI
    public interface ISystemClock
    {
        DateTime UtcNow { get; }
    }

    public sealed class SystemClock : ISystemClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }

    // Background service registered only in the Generic Host example
    public sealed class TimeReporterBackgroundService : BackgroundService
    {
        private readonly ILogger<TimeReporterBackgroundService> _logger;
        private readonly ISystemClock _clock;

        public TimeReporterBackgroundService(ILogger<TimeReporterBackgroundService> logger, ISystemClock clock)
        {
            _logger = logger;
            _clock = clock;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TimeReporter started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Background tick at {Time}", _clock.UtcNow);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            _logger.LogInformation("TimeReporter stopping.");
        }
    }
}

/*
EXPLANATION SUMMARY

Generic Host (recommended):
- Entry: Host.CreateDefaultBuilder
- Unified abstraction for web apps + background workers.
- Supports hosting HTTP + long-running background tasks (AddHostedService).
- Extensible configuration & logging pipelines in one place.
- Used implicitly by WebApplication / minimal APIs / Razor Pages in modern ASP.NET Core.

Legacy Web Host:
- Entry: WebHost / WebHostBuilder
- Focused on web server concerns only (HTTP pipeline).
- Pre-.NET Core 3 style. Largely superseded by Generic Host.
- Lacks the unified model for non-HTTP background services (you can still add them, but Generic Host is cleaner).

How to run:
dotnet run              -> Generic Host (shows background log ticks + HTTP endpoint)
dotnet run -- --webhost -> Legacy Web Host (HTTP only)

*/
