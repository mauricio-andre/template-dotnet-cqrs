
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace CqrsProject.OpenTelemetry.Extensions;

public static class OpenTelemetryBuilderExtension
{
    public static OpenTelemetryBuilder AddOpenTelemetryTracingProvider(
        this OpenTelemetryBuilder openTelemetryBuilder,
        IHostApplicationBuilder builder)
    {
        var tracingSection = builder.Configuration.GetSection("OpenTelemetry:Tracing");

        if (!tracingSection.GetValue<bool>("Enable"))
            return openTelemetryBuilder;

        var serviceName = builder.Configuration.GetValue<string>("ServiceName")!;
        openTelemetryBuilder.WithTracing(tracing =>
        {
            tracing
                .AddSource(serviceName)
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true);

            string? endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

            if (!string.IsNullOrEmpty(endpoint))
                tracing.AddOtlpExporter();
        });

        return openTelemetryBuilder;
    }

    public static OpenTelemetryBuilder AddOpenTelemetryMetricsProvider(
        this OpenTelemetryBuilder openTelemetryBuilder,
        IHostApplicationBuilder builder)
    {
        var metricsSection = builder.Configuration.GetSection("OpenTelemetry:Metrics");

        if (!metricsSection.GetValue<bool>("Enable"))
            return openTelemetryBuilder;

        openTelemetryBuilder.WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel");

            string? endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

            if (!string.IsNullOrEmpty(endpoint))
                metrics.AddOtlpExporter();
        });

        return openTelemetryBuilder;
    }
}
