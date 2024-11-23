namespace CqrsProject.Common.Loggers;

public static class LoggerPropertiesHelper
{
    public static KeyValuePair<string, object?>[] DefaultPropertyList() =>
        [
            new KeyValuePair<string, object?>("tenantId", "")
        ];

    public static KeyValuePair<string, object?>[] ScopeObjectStructuring(object value)
    {
        if (value is TenantLoggerRecord tenantLoggerRecord)
            return [
                new KeyValuePair<string, object?>("tenantId", tenantLoggerRecord.TenantId?.ToString() ?? string.Empty)
            ];

        return [];
    }
}
