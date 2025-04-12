using CqrsProject.App.DbMigrator;
using CqrsProject.App.DbMigrator.Interfaces;
using CqrsProject.App.DbMigrator.Services;
using CqrsProject.App.DbMigratorTest.Database;
using CqrsProject.App.DbMigratorTest.Services;
using CqrsProject.Common.Diagnostics;
using CqrsProject.Common.Providers.KeyVaults.Interfaces;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Core.Tenants.Services;
using CqrsProject.Postgres.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CqrsProject.CustomStringLocalizer.Extensions;

namespace CqrsProject.App.DbMigratorTest;

public static class DbMigratorHostFactory
{
    public static IHost DbMigratorHost(Action<IServiceCollection>? configureServices = null)
    {
        var builder = Host.CreateApplicationBuilder();

        var postgresContainer = new PostgresDocker();
        var configValues = new Dictionary<string, string?>
        {
            { "ConnectionStrings:AdministrationDbContext", postgresContainer.GetConnectionStringAdministration() },
            { "ConnectionStrings:CoreDbContext", postgresContainer.GetConnectionStringHost() }
        };

        builder.Configuration.AddInMemoryCollection(configValues);

        builder.Services
            .AddPostgresAdministrationDbContext()
            .AddPostgresCoreDbContext()
            .AddScoped<ITenantConnectionProvider, TenantConnectionProvider>()
            .AddScoped<ICurrentTenant, CurrentTenant>()
            .AddScoped<IDbMigratorService, DbMigratorService>()
            .AddSingleton(_ => new CqrsProjectActivitySource("DbMigratorTest"))
            .AddSingleton<IDatabaseDocker>(postgresContainer)
            .AddSingleton<IKeyVaultService, KeyVaultService>();

        // configuration identity
        builder.Services
            .AddIdentityCore<User>()
            .AddEntityFrameworkStores<AdministrationDbContext>();

        builder.Services.AddCustomStringLocalizerProvider();

        builder.Services.AddHostedService<DbMigratorBackgroundService>();

        configureServices?.Invoke(builder.Services);

        var app = builder.Build();

        return app;
    }
}
