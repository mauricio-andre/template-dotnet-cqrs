using Microsoft.Extensions.Logging.Console;

namespace CqrsProject.CustomConsoleFormatter.Options;

public sealed class JsonCustomConsoleFormatterOptions : JsonConsoleFormatterOptions
{
    public static readonly string FormatterName = "JsonCustom";
    public string EnvironmentName { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public bool IncludeDetails { get; set; }
    new public string? TimestampFormat { get; set; } = "yyyy-MM-dd\"T\"HH:mm:ss.fffzzz";
}
