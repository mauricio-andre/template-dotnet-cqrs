
using System.Linq.Dynamic.Core;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CqrsProject.Core.Tenants;

public class CurrentTenant : ICurrentTenant
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IMemoryCache _memoryCache;
    private Guid? _tenantId;

    public CurrentTenant(AdministrationDbContext administrationDbContext, IMemoryCache memoryCache)
    {
        _administrationDbContext = administrationDbContext;
        _memoryCache = memoryCache;
    }

    public Guid? GetCurrentTenantId() => _tenantId;
    public void SetCurrentTenantId(Guid? tenantId) => _tenantId = tenantId;
    public bool IsHost() => _tenantId == null;
    public async ValueTask<string?> GetConnectionKeyNameAsync(string connectionName)
    {
        if (IsHost())
            return null;

        var keyName = await _memoryCache.GetOrCreateAsync(
            string.Format(CacheKeys.TenantConnectionStringKeyName, _tenantId, connectionName),
            factory =>
            {
                factory.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);

                return _administrationDbContext.TenantConnectionStrings
                    .AsNoTracking()
                    .Where(tenantConnection => tenantConnection.TenantId == _tenantId
                        && tenantConnection.ConnectionName == connectionName)
                    .Select(tenantConnection => tenantConnection.KeyName)
                    .FirstOrDefaultAsync();
            }
        );

        return keyName;
    }
}
