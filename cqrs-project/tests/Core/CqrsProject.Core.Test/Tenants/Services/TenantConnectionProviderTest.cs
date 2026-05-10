using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Common.Providers.KeyVaults.Interfaces;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Caches;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Core.Tenants.Services;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Test.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using NSubstitute;

namespace CqrsProject.Core.Test.Tenants.Services;

public class TenantConnectionProviderTest
{
    [Fact(DisplayName = "Should return the host connection string when current tenant is host")]
    public void GivenHostTenant_WhenGettingConnectionString_ThenReturnConfigurationValue()
    {
        IConfiguration configuration = BuildConfiguration("HostConnection", "Server=host;");
        ICurrentTenant currentTenant = Substitute.For<ICurrentTenant>();
        currentTenant.IsHost().Returns(true);
        IKeyVaultService keyVaultService = Substitute.For<IKeyVaultService>();
        TenantConnectionProvider provider = CreateProvider(configuration, currentTenant, keyVaultService);

        string? connectionString = provider.GetConnectionStringToCurrentTenant("HostConnection");

        Assert.Equal("Server=host;", connectionString);
    }

    [Fact(DisplayName = "Should return the cached connection string when tenant is not host")]
    public void GivenTenantScopeAndCachedValue_WhenGettingConnectionString_ThenReturnCachedValue()
    {
        Guid tenantId = Guid.NewGuid();
        ConnectionStringCache.Instance.SetConnectionString(tenantId, "TenantConnection", "Server=tenant;");

        IConfiguration configuration = BuildConfiguration("TenantConnection", "Server=host;");
        ICurrentTenant currentTenant = Substitute.For<ICurrentTenant>();
        currentTenant.IsHost().Returns(false);
        currentTenant.GetCurrentTenantId().Returns(tenantId);
        IKeyVaultService keyVaultService = Substitute.For<IKeyVaultService>();
        TenantConnectionProvider provider = CreateProvider(configuration, currentTenant, keyVaultService);

        string? connectionString = provider.GetConnectionStringToCurrentTenant("TenantConnection");

        Assert.Equal("Server=tenant;", connectionString);
        ConnectionStringCache.Instance.RemoveConnectionString(tenantId, "TenantConnection");
    }

    [Fact(DisplayName = "Should load tenant connection strings from the database into the cache")]
    public async Task GivenDatabaseRecords_WhenLoadingConnectionStrings_ThenPopulateCache()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        Guid activeTenantId = Guid.NewGuid();
        Guid deletedTenantId = Guid.NewGuid();

        await TenantsTestData.AddTenantAsync(dbContext, activeTenantId, "Active Tenant");
        await TenantsTestData.AddTenantAsync(dbContext, deletedTenantId, "Deleted Tenant", true);
        await TenantsTestData.AddTenantConnectionStringAsync(dbContext, activeTenantId, "Default", "ACTIVE_KEY");
        await TenantsTestData.AddTenantConnectionStringAsync(dbContext, deletedTenantId, "Default", "DELETED_KEY");

        IKeyVaultService keyVaultService = Substitute.For<IKeyVaultService>();
        keyVaultService.GetKeyValueAsync("ACTIVE_KEY").Returns("Server=active;");

        TenantConnectionProvider provider = CreateProvider(
            BuildConfiguration("Default", "Server=host;"),
            Substitute.For<ICurrentTenant>(),
            keyVaultService,
            context.AdministrationDbContextFactory);

        await provider.LoadAllConnectionStringAsync();

        Assert.Equal(
            "Server=active;",
            ConnectionStringCache.Instance.GetConnectionString(activeTenantId, "Default"));
        Assert.Null(ConnectionStringCache.Instance.GetConnectionString(deletedTenantId, "Default"));

        ConnectionStringCache.Instance.RemoveConnectionString(activeTenantId, "Default");
    }

    [Fact(DisplayName = "Should include a tenant connection string using the key vault value")]
    public async Task GivenKeyVaultValue_WhenIncludingConnectionString_ThenStoreItInCache()
    {
        IConfiguration configuration = BuildConfiguration("Default", "Server=host;");
        ICurrentTenant currentTenant = Substitute.For<ICurrentTenant>();
        IKeyVaultService keyVaultService = Substitute.For<IKeyVaultService>();
        keyVaultService.GetKeyValueAsync("ACTIVE_KEY").Returns("Server=active;");
        TenantConnectionProvider provider = CreateProvider(configuration, currentTenant, keyVaultService);
        Guid tenantId = Guid.NewGuid();

        await provider.IncludeConnectionStringAsync(tenantId, "Default", "ACTIVE_KEY");

        Assert.Equal("Server=active;", ConnectionStringCache.Instance.GetConnectionString(tenantId, "Default"));
        ConnectionStringCache.Instance.RemoveConnectionString(tenantId, "Default");
    }

    [Fact(DisplayName = "Should reject missing key vault values when including a connection string")]
    public async Task GivenMissingKeyVaultValue_WhenIncludingConnectionString_ThenThrowException()
    {
        IConfiguration configuration = BuildConfiguration("Default", "Server=host;");
        ICurrentTenant currentTenant = Substitute.For<ICurrentTenant>();
        IKeyVaultService keyVaultService = Substitute.For<IKeyVaultService>();
        TenantConnectionProvider provider = CreateProvider(configuration, currentTenant, keyVaultService);

        await Assert.ThrowsAsync<ConnectionStringKeyNotFoundException>(
            () => provider.IncludeConnectionStringAsync(Guid.NewGuid(), "Default", "MISSING_KEY"));
    }

    [Fact(DisplayName = "Should invalidate a cached tenant connection string")]
    public void GivenCachedConnectionString_WhenInvalidating_ThenRemoveItFromCache()
    {
        Guid tenantId = Guid.NewGuid();
        ConnectionStringCache.Instance.SetConnectionString(tenantId, "Default", "Server=tenant;");

        TenantConnectionProvider provider = CreateProvider(
            BuildConfiguration("Default", "Server=host;"),
            Substitute.For<ICurrentTenant>(),
            Substitute.For<IKeyVaultService>());

        provider.InvalidateConnectionString(tenantId, "Default");

        Assert.Null(ConnectionStringCache.Instance.GetConnectionString(tenantId, "Default"));
    }

    private static TenantConnectionProvider CreateProvider(
        IConfiguration configuration,
        ICurrentTenant currentTenant,
        IKeyVaultService keyVaultService,
        IDbContextFactory<AdministrationDbContext>? dbContextFactory = null)
    {
        return new TenantConnectionProvider(
            configuration,
            currentTenant,
            Substitute.For<IServiceProvider>(),
            dbContextFactory ?? Substitute.For<IDbContextFactory<AdministrationDbContext>>(),
            Substitute.For<IStringLocalizer<CqrsProjectResource>>(),
            keyVaultService);
    }

    private static IConfiguration BuildConfiguration(string connectionName, string connectionString)
    {
        IConfigurationSection connectionStringsSection = Substitute.For<IConfigurationSection>();
        IConfiguration configuration = Substitute.For<IConfiguration>();

        configuration.GetSection("ConnectionStrings").Returns(connectionStringsSection);
        connectionStringsSection[connectionName].Returns(connectionString);

        return configuration;
    }
}

