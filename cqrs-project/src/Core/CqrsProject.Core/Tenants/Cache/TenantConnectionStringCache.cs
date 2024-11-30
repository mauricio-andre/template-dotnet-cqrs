namespace CqrsProject.Core.Tenants.Cache;

public class ConnectionStringCache
{
    private static readonly ConnectionStringCache _instance = new ConnectionStringCache();

    public static ConnectionStringCache Instance => _instance;

    private readonly Dictionary<string, string> _connectionStrings;

    private ConnectionStringCache()
    {
        _connectionStrings = new Dictionary<string, string>();
    }

    public static string ComposeCacheKey(Guid tenantId, string connectionName)
        => string.Concat(tenantId, connectionName);

    public void SetConnections(Dictionary<string, string> connections)
    {
        foreach (var connection in connections)
        {
            _connectionStrings[connection.Key] = connection.Value;
        }
    }

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
}
