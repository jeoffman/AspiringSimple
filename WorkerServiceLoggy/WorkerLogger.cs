using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerServiceLoggy;

public class WorkerLogger : BackgroundService
{
    private readonly ILogger<WorkerLogger> _logger;

    public WorkerLogger(ILogger<WorkerLogger> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ActivitySource activitySource = new(nameof(WorkerLogger), "1.0.0");

        Meter meter = new Meter($"{nameof(WorkerLogger)}.counter");
        Counter<int> counting = meter.CreateCounter<int>($"{nameof(WorkerLogger)}.counting");

        while (!stoppingToken.IsCancellationRequested)
        {
            using Activity? activity = activitySource.StartActivity();

            counting.Add(1);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("WorkerLogger running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
