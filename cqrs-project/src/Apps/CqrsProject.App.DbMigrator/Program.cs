using CqrsProject.App.DbMigrator;
using CqrsProject.App.DbMigrator.Interfaces;
using CqrsProject.App.DbMigrator.Loggers;
using CqrsProject.App.DbMigrator.Services;
using CqrsProject.Common.Diagnostics;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Core.Tenants.Services;
using CqrsProject.CustomConsoleFormatter.Extensions;
using CqrsProject.OpenTelemetry.Extensions;
using CqrsProject.Postegre.Extensions;
using Microsoft.EntityFrameworkCore;
using CqrsProject.Core.Tenants.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddPostegreAdministrationDbContext(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("AdministrationDbContext"));
    })
    .AddPostegreCoreDbContext()
    .AddScoped<ITenantConnectionProvider, TenantConnectionProvider>()
    .AddScoped<ICurrentTenant, CurrentTenant>()
    .AddScoped<IDbMigratorService, DbMigratorService>()
    .AddSingleton(_ => new CqrsProjectActivitySource(builder.Configuration.GetValue<string>("ServiceName")!));

// configuration identity
builder.Services
    .AddIdentityCore<User>()
    .AddEntityFrameworkStores<AdministrationDbContext>();

// Configure providers
builder.Services.AddCustomConsoleFormatterProvider<LoggerPropertiesService>();
builder.AddOpenTelemetryProvider();

builder.Services.AddHostedService<DbMigratorBackgroundService>();

var app = builder.Build();

app.LoadMultiTenantConnections();

await app.RunAsync();
