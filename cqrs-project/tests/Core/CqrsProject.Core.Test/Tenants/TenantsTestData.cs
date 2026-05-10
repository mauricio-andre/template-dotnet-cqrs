using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Tenants;

internal static class TenantsTestData
{
    public static async Task<Tenant> AddTenantAsync(
        AdministrationDbContext dbContext,
        Guid? id = null,
        string? name = null,
        bool isDeleted = false)
    {
        Tenant tenant = new Tenant
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? $"Tenant-{Guid.NewGuid():N}",
            IsDeleted = isDeleted
        };

        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.SaveChangesAsync();

        return tenant;
    }

    public static async Task<TenantConnectionString> AddTenantConnectionStringAsync(
        AdministrationDbContext dbContext,
        Guid tenantId,
        string connectionName,
        string keyName)
    {
        TenantConnectionString tenantConnectionString = new TenantConnectionString
        {
            TenantId = tenantId,
            ConnectionName = connectionName,
            KeyName = keyName
        };

        await dbContext.TenantConnectionStrings.AddAsync(tenantConnectionString);
        await dbContext.SaveChangesAsync();

        return tenantConnectionString;
    }
}
