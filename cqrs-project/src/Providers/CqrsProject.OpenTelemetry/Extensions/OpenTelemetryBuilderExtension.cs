
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Exporter;
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

            if (tracingSection.GetValue<bool>("ConsoleExporter"))
                tracing.AddConsoleExporter();

            var endpoint = tracingSection.GetValue<string>("OtlpExporter:Endpoint");
            if (!string.IsNullOrEmpty(endpoint))
                tracing.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(endpoint);
                    options.Protocol = tracingSection.GetValue<string>("OtlpExporter:Protocol")?.ToLower() == "httpprotobuf"
                        ? OtlpExportProtocol.HttpProtobuf
                        : OtlpExportProtocol.Grpc;
                });
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
                .AddHttpClientInstrumentation();

            if (metricsSection.GetValue<bool>("ConsoleExporter"))
                metrics.AddConsoleExporter();

            var endpoint = metricsSection.GetValue<string>("OtlpExporter:Endpoint");
            if (!string.IsNullOrEmpty(endpoint))
                metrics.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(endpoint);
                    options.Protocol = metricsSection.GetValue<string>("OtlpExporter:Protocol")?.ToLower() == "httpprotobuf"
                        ? OtlpExportProtocol.HttpProtobuf
                        : OtlpExportProtocol.Grpc;
                });
        });

        return openTelemetryBuilder;
    }
}
