namespace CqrsProject.CustomConsoleFormatter.Interfaces;

public interface ILoggerPropertiesService
{
    string GetAppUser();

    KeyValuePair<string, object?>[] DefaultPropertyList();

    KeyValuePair<string, object?>[] ScopeObjectStructuring(object value);
}
