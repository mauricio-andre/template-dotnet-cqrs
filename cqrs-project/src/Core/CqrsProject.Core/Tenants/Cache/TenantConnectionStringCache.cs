using System.Collections.Concurrent;

namespace CqrsProject.Core.Tenants.Caches;

public class ConnectionStringCache
{
    private static readonly ConnectionStringCache _instance = new ConnectionStringCache();

    public static ConnectionStringCache Instance => _instance;

    private readonly ConcurrentDictionary<string, string> _connectionStrings;

    private ConnectionStringCache()
    {
        _connectionStrings = new ConcurrentDictionary<string, string>();
    }

    public static string ComposeCacheKey(Guid tenantId, string connectionName)
        => string.Concat(tenantId, connectionName);

    public void SetConnectionString(Guid tenantId, string connectionName, string value)
    {
        var key = ComposeCacheKey(tenantId, connectionName);
        _connectionStrings[key] = value;
    }

    public string? GetConnectionString(Guid tenantId, string connectionName)
    {
        var key = ComposeCacheKey(tenantId, connectionName);
        return _connectionStrings.TryGetValue(key, out var value) ? value : null;
    }

    public void RemoveConnectionString(Guid tenantId, string connectionName)
    {
        var key = ComposeCacheKey(tenantId, connectionName);
        _connectionStrings.Remove(key, out _);
    }
}
