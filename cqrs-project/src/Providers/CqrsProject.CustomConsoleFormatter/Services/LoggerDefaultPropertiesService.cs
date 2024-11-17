using CqrsProject.CustomConsoleFormatter.Interfaces;

namespace CqrsProject.CustomConsoleFormatter.Services;

public class LoggerDefaultPropertiesService : ILoggerPropertiesService
{
    public string GetAppUser() => "Unknown";
    public KeyValuePair<string, object?>[] DefaultPropertyList() => [];
    public KeyValuePair<string, object?>[] ScopeObjectStructuring(object value) => [];
}
