using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyInjection;

public sealed class MyHostedService : IHostedService, IDisposable
{
    private readonly ILogger<MyHostedService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private CancellationTokenSource? _stoppingCts;
    private Task? _backgroundTask;

    public MyHostedService(
        ILogger<MyHostedService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MyHostedService starting.");
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _backgroundTask = RunAsync(_stoppingCts.Token);
        return Task.CompletedTask;
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        var iteration = 0;

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                iteration++;
                using var scope = _scopeFactory.CreateScope();
                var reportService = scope.ServiceProvider.GetRequiredService<RequestReportService>();
                var report = reportService.CreateReport();


                _logger.LogInformation(
                    "Iteration {Iteration}: Transient1={Transient1} Transient2={Transient2} Scoped={Scoped} Singleton={Singleton} ReportSvc={ReportSvc} UtcNow={UtcNow:O}",
                    iteration,
                    report.Transient1,
                    report.Transient2,
                    report.Scoped,
                    report.Singleton,
                    report.ReportServiceInstance,
                    report.UtcNow);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("MyHostedService cancellation requested.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in MyHostedService loop.");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_backgroundTask is null)
            return;

        _logger.LogInformation("MyHostedService stopping.");

        try
        {
            _stoppingCts?.Cancel();
        }
        catch { /* ignore */ }

        await Task.WhenAny(_backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken));
    }

    public void Dispose()
    {
        _stoppingCts?.Dispose();
    }
}
