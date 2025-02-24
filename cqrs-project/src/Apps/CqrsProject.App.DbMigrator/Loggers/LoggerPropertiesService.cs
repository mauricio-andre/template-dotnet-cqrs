using CqrsProject.Common.Loggers;
using CqrsProject.CustomConsoleFormatter.Interfaces;

namespace CqrsProject.App.DbMigrator.Loggers;

public class LoggerPropertiesService : ILoggerPropertiesService
{
    public string GetAppUser()
    {
        return "DbMigrator";
    }

    public KeyValuePair<string, object?>[] DefaultPropertyList() =>
        new TenantLoggerRecord().ToArray();

    public KeyValuePair<string, object?>[] ScopeObjectStructuring(object value)
    {
        if (value is TenantLoggerRecord tenantLoggerRecord)
            return tenantLoggerRecord.ToArray();

        return [];
    }
}
