using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WorkerServiceLoggy;

public class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddHostedService<WorkerLogger>();
        builder.Services.AddLogging();

        builder.Logging.AddDebug();
        builder.Logging.AddOpenTelemetry((configure) =>
        {
            configure.AddOtlpExporter();
            configure.IncludeFormattedMessage = true;   //should be done in config/json, but its not being picked up
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(
                    serviceName: nameof(WorkerLogger),
                    serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown")
            )
            .WithTracing(tracing =>
            {
                tracing.AddSource("*"); //or just WorkerLogger if I was so inclined
                tracing.AddOtlpExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics.AddMeter("*"); //or just WorkerLogger.counter if I was so inclined
                metrics.AddOtlpExporter();
            });

        var host = builder.Build();
        host.Run();
    }
}
