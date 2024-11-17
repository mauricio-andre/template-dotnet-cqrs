using CqrsProject.CustomConsoleFormatter.Formatters;
using CqrsProject.CustomConsoleFormatter.Interfaces;
using CqrsProject.CustomConsoleFormatter.Options;
using CqrsProject.CustomConsoleFormatter.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CqrsProject.CustomConsoleFormatter.Extensions;

public static class CustomConsoleFormatterServiceCollectionExtension
{
    public static IServiceCollection AddCustomConsoleFormatterProvider(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerPropertiesService, LoggerDefaultPropertiesService>();
        services.AddLogging(builder => builder.AddCustomFormatters());
        return services;
    }

    public static IServiceCollection AddCustomConsoleFormatterProvider<TLoggerProperties>(this IServiceCollection services)
    where TLoggerProperties : class, ILoggerPropertiesService
    {
        services.AddSingleton<ILoggerPropertiesService, TLoggerProperties>();
        services.AddLogging(builder => builder.AddCustomFormatters());
        return services;
    }

    public static ILoggingBuilder AddCustomFormatters(this ILoggingBuilder builder) =>
        builder
            .AddConsole()
            .AddConsoleFormatter<JsonCustomConsoleFormatter, JsonCustomConsoleFormatterOptions>();
}
