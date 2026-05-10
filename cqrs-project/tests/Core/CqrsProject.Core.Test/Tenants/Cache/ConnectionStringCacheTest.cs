using CqrsProject.Core.Tenants.Caches;

namespace CqrsProject.Core.Test.Tenants.Cache;

public class ConnectionStringCacheTest
{
    [Fact(DisplayName = "Should compose a stable cache key from tenant id and connection name")]
    public void GivenTenantAndConnectionName_WhenComposingKey_ThenReturnConcatenatedKey()
    {
        Guid tenantId = Guid.NewGuid();

        string key = ConnectionStringCache.ComposeCacheKey(tenantId, "Default");

        Assert.Equal($"{tenantId}Default", key);
    }

    [Fact(DisplayName = "Should store, read and remove a tenant connection string")]
    public void GivenTenantConnectionString_WhenUsingCache_ThenPersistAndRemoveIt()
    {
        ConnectionStringCache cache = ConnectionStringCache.Instance;
        Guid tenantId = Guid.NewGuid();

        cache.SetConnectionString(tenantId, "Default", "Host=localhost;");

        Assert.Equal("Host=localhost;", cache.GetConnectionString(tenantId, "Default"));

        cache.RemoveConnectionString(tenantId, "Default");

        Assert.Null(cache.GetConnectionString(tenantId, "Default"));
    }
}
