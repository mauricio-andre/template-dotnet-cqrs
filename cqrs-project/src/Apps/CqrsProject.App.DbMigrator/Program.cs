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

var builder = Host.CreateApplicationBuilder(args);

// Configure additional appsettings
builder.Configuration
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
    .AddEnvironmentVariables();

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

var app = builder.Build();

await app.StartAsync();

var activitySource = app.Services.GetRequiredService<CqrsProjectActivitySource>();
using (var activity = activitySource.ActivitySourceDefault.StartActivity("RunDbMigrator"))
    await app.Services
        .GetRequiredService<IDbMigratorService>()
        .RunMigrateAsync();
