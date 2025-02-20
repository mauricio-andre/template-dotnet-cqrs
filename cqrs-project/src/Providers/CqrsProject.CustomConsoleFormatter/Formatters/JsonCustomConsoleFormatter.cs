using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using CqrsProject.CustomConsoleFormatter.Interfaces;
using CqrsProject.CustomConsoleFormatter.Microsoft.Text.Json;
using CqrsProject.CustomConsoleFormatter.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace CqrsProject.CustomConsoleFormatter.Formatters;

internal sealed class JsonCustomConsoleFormatter : ConsoleFormatter, IDisposable
{
    internal JsonCustomConsoleFormatterOptions FormatterOptions { get; set; }
    private readonly IDisposable? _optionsReloadToken;
    private readonly ILoggerPropertiesService _loggerPropertiesService;
    private readonly KeyValuePair<string, object?>[] _defaultPropertyList;
    private readonly string _hostName = Dns.GetHostName();

    public JsonCustomConsoleFormatter(
        IOptionsMonitor<JsonCustomConsoleFormatterOptions> options,
        ILoggerPropertiesService loggerPropertiesService) : base(JsonCustomConsoleFormatterOptions.FormatterName)
    {
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        _loggerPropertiesService = loggerPropertiesService;
        _defaultPropertyList = _loggerPropertiesService.DefaultPropertyList();
    }

    [MemberNotNull(nameof(FormatterOptions))]
    private void ReloadLoggerOptions(JsonCustomConsoleFormatterOptions options) =>
        FormatterOptions = options;

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
            return;

        try
        {
            WriteJson(logEntry, scopeProvider, textWriter, message);
        }
        catch (Exception exception)
        {
            HandleException(message, exception, textWriter);
        }
    }

    private static void HandleException(string message, Exception exception, TextWriter textWriter)
    {
        textWriter.WriteLine(message);
        textWriter.WriteLine("An error occurred while serializing custom log json");
        textWriter.WriteLine(exception.Message);
        textWriter.WriteLine(exception.StackTrace);
    }

    private void WriteJson<TState>(
        LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter,
        string message)
    {
        const int DefaultBufferSize = 1024;
        using (var output = new PooledByteBufferWriter(DefaultBufferSize))
        {
            using (var writer = new Utf8JsonWriter(output, FormatterOptions.JsonWriterOptions))
            {
                writer.WriteStartObject();
                writer.WriteString("timestamp", GetTimestamp());
                writer.WriteString("environmentName", FormatterOptions.EnvironmentName);
                writer.WriteString("serviceName", FormatterOptions.ServiceName);
                writer.WriteString("hostName", _hostName);
                writer.WriteString("userProcess", Environment.UserName);
                writer.WriteNumber("eventId", logEntry.EventId.Id);
                writer.WriteNumber("thread", Thread.CurrentThread.ManagedThreadId);

                writer.WriteStartObject("trace");
                writer.WriteString("id", Activity.Current?.TraceId.ToString());
                writer.WriteEndObject();
                writer.WriteStartObject("transaction");
                writer.WriteString("id", Activity.Current?.Id);
                writer.WriteEndObject();
                writer.WriteStartObject("span");
                writer.WriteString("id", Activity.Current?.SpanId.ToString());
                writer.WriteEndObject();

                writer.WriteString("category", logEntry.Category);
                writer.WriteString("logLevel", GetLogLevelString(logEntry.LogLevel));
                writer.WriteString("appUser", _loggerPropertiesService.GetAppUser());
                writer.WriteString("message", message);
                writer.WriteString("exceptionMessage", logEntry.Exception?.Message ?? "");
                writer.WriteString("exceptionTrace", GetExceptionStackTrace(logEntry.Exception));
                WriteStateInformation(logEntry, writer);
                var unresolvedScopeList = WriteDetailInformation(writer, scopeProvider);
                WriteScopeInformation(writer, scopeProvider, unresolvedScopeList);
                writer.WriteEndObject();
                writer.Flush();
            }

            var messageBytes = output.WrittenMemory.Span;
            var logMessageBuffer = ArrayPool<char>.Shared.Rent(Encoding.UTF8.GetMaxCharCount(messageBytes.Length));

            try
            {
                var charsWritten = Encoding.UTF8.GetChars(messageBytes, logMessageBuffer);
                textWriter.Write(logMessageBuffer, 0, charsWritten);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(logMessageBuffer);
            }
        }

        textWriter.Write(Environment.NewLine);
    }

    private string GetTimestamp()
    {
        var dateTimeOffset = FormatterOptions.UseUtcTimestamp
            ? DateTimeOffset.UtcNow
            : DateTimeOffset.Now;

        return string.IsNullOrEmpty(FormatterOptions.TimestampFormat)
            ? dateTimeOffset.ToString()
            : dateTimeOffset.ToString(FormatterOptions.TimestampFormat);
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "Trace",
            LogLevel.Debug => "Debug",
            LogLevel.Information => "Information",
            LogLevel.Warning => "Warning",
            LogLevel.Error => "Error",
            LogLevel.Critical => "Critical",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    private static string GetExceptionStackTrace(Exception? exception)
    {
        return exception?.InnerException != null
            ? string.Concat(
                exception.StackTrace,
                Environment.NewLine,
                exception.InnerException?.Message,
                Environment.NewLine,
                GetExceptionStackTrace(exception.InnerException)
            )
            : exception?.StackTrace
            ?? "";
    }

    private static void WriteStateInformation<TState>(LogEntry<TState> logEntry, Utf8JsonWriter writer)
    {
        if (logEntry.State is not null)
        {
            writer.WriteStartObject("state");
            writer.WriteString("message", logEntry.State?.ToString());

            if (logEntry.State is IReadOnlyList<KeyValuePair<string, object?>> stateProperties)
            {
                foreach (KeyValuePair<string, object?> item in stateProperties)
                {
                    WriteItem(writer, item);
                }
            }

            writer.WriteEndObject();
        }
    }

    private List<object> WriteDetailInformation(Utf8JsonWriter writer, IExternalScopeProvider? scopeProvider)
    {
        var unresolvedScopeList = new List<object>();
        if (FormatterOptions.IncludeDetails && scopeProvider != null)
        {
            var keyUsedList = new List<string>();
            writer.WriteStartObject("appDetails");
            writer.WriteStartObject(GetNormalizedAppName());
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (scope != null)
                {
                    var results = _loggerPropertiesService.ScopeObjectStructuring(scope);
                    for (int i = 0; i < results.Length; i++)
                    {
                        keyUsedList.Add(results[i].Key);
                        WhileAppDetailProperties(results[i], state);
                    }

                    if (results.Length <= 0)
                        unresolvedScopeList.Add(scope);
                }
            }, writer);

            WriteDetailDefaultInformation(keyUsedList.ToArray(), writer);

            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        return unresolvedScopeList;
    }

    private string GetNormalizedAppName() => string.Concat(
        char.ToLowerInvariant(FormatterOptions.ServiceName[0]),
        FormatterOptions.ServiceName.Substring(1)
    );

    private static void WhileAppDetailProperties(KeyValuePair<string, object?> element, Utf8JsonWriter state)
    {
        if (element.Value is IEnumerable<KeyValuePair<string, object?>> objectProperties)
        {
            state.WriteStartObject(element.Key);
            foreach (KeyValuePair<string, object?> item in objectProperties)
            {
                WhileAppDetailProperties(item, state);
            }
            state.WriteEndObject();
        }
        else
        {
            WriteItem(state, element);
        }
    }

    private void WriteDetailDefaultInformation(string[] keyUsedList, Utf8JsonWriter writer)
    {
        for (int i = 0; i < _defaultPropertyList.Length; i++)
            if (Array.IndexOf(keyUsedList, _defaultPropertyList[i].Key) == -1)
                WhileAppDetailProperties(_defaultPropertyList[i], writer);
    }

    private void WriteScopeInformation(
        Utf8JsonWriter writer,
        IExternalScopeProvider? scopeProvider,
        List<object> unresolvedScopeList)
    {
        if (FormatterOptions.IncludeScopes && scopeProvider != null)
        {
            writer.WriteStartArray("scopes");
            // If Include Details is active, use unresolvedScopeList to create scopes
            if (FormatterOptions.IncludeDetails)
            {
                foreach (var scope in unresolvedScopeList)
                    WhileScope(scope, writer);
            }
            else
            {
                scopeProvider.ForEachScope((scope, state) =>
                {
                    WhileScope(scope, state);
                }, writer);
            }

            writer.WriteEndArray();
        }
    }

    private static void WhileScope(object? scope, Utf8JsonWriter state)
    {
        if (scope is IEnumerable<KeyValuePair<string, object?>> scopeItems)
        {
            state.WriteStartObject();
            state.WriteString("message", scope.ToString());
            foreach (KeyValuePair<string, object?> item in scopeItems)
            {
                WriteItem(state, item);
            }

            state.WriteEndObject();
        }
        else
        {
            state.WriteStringValue(ToInvariantString(scope));
        }
    }

    private static void WriteItem(Utf8JsonWriter writer, KeyValuePair<string, object?> item)
    {
        var key = item.Key;
        switch (item.Value)
        {
            case bool boolValue:
                writer.WriteBoolean(key, boolValue);
                break;
            case byte byteValue:
                writer.WriteNumber(key, byteValue);
                break;
            case sbyte sbyteValue:
                writer.WriteNumber(key, sbyteValue);
                break;
            case char charValue:
                writer.WriteString(key, MemoryMarshal.CreateSpan(ref charValue, 1));
                break;
            case decimal decimalValue:
                writer.WriteNumber(key, decimalValue);
                break;
            case double doubleValue:
                writer.WriteNumber(key, doubleValue);
                break;
            case float floatValue:
                writer.WriteNumber(key, floatValue);
                break;
            case int intValue:
                writer.WriteNumber(key, intValue);
                break;
            case uint uintValue:
                writer.WriteNumber(key, uintValue);
                break;
            case long longValue:
                writer.WriteNumber(key, longValue);
                break;
            case ulong ulongValue:
                writer.WriteNumber(key, ulongValue);
                break;
            case short shortValue:
                writer.WriteNumber(key, shortValue);
                break;
            case ushort ushortValue:
                writer.WriteNumber(key, ushortValue);
                break;
            case null:
                writer.WriteNull(key);
                break;
            default:
                writer.WriteString(key, ToInvariantString(item.Value));
                break;
        }
    }

    private static string? ToInvariantString(object? obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}
