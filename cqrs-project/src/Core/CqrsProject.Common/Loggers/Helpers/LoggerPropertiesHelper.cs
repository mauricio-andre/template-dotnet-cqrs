namespace CqrsProject.Common.Loggers;

public static class LoggerPropertiesHelper
{
    public static KeyValuePair<string, object?>[] DefaultPropertyList() =>
        new TenantLoggerRecord().ToArray();

    public static KeyValuePair<string, object?>[] ScopeObjectStructuring(object value)
    {
        if (value is TenantLoggerRecord tenantLoggerRecord)
            return tenantLoggerRecord.ToArray();

        return [];
    }
}
