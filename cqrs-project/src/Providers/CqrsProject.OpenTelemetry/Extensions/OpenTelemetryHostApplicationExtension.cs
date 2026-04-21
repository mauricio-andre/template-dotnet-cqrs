
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CqrsProject.OpenTelemetry.Extensions;

public static class OpenTelemetryHostApplicationExtension
{
    public static IHostApplicationBuilder AddOpenTelemetryProvider(this IHostApplicationBuilder builder)
    {
        builder
            .AddOpenTelemetryLoggingProvider()
            .AddOpenTelemetryResource()
            .AddOpenTelemetryTracingProvider(builder)
            .AddOpenTelemetryMetricsProvider(builder);

        return builder;
    }

    private static OpenTelemetryBuilder AddOpenTelemetryResource(
        this IHostApplicationBuilder builder)
    {
        var serviceName = builder.Configuration.GetValue<string>("ServiceName")!;

        return builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName)
                .AddEnvironmentVariableDetector()
                .AddTelemetrySdk());
    }

    public static IHostApplicationBuilder AddOpenTelemetryLoggingProvider(this IHostApplicationBuilder builder)
    {
        var loggingSection = builder.Configuration.GetSection("OpenTelemetry:Logging");

        if (!loggingSection.GetValue<bool>("Enable"))
            return builder;

        var serviceName = builder.Configuration.GetValue<string>("ServiceName")!;
        builder.Logging.AddOpenTelemetry(configure =>
        {
            configure.IncludeScopes = loggingSection.GetValue<bool>("IncludeScopes");
            configure.ParseStateValues = loggingSection.GetValue<bool>("ParseStateValues");
            configure.IncludeFormattedMessage = loggingSection.GetValue<bool>("IncludeFormattedMessage");
            configure.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));

            string? endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

            if (!string.IsNullOrEmpty(endpoint))
                configure.AddOtlpExporter();
        });

        return builder;
    }
}
