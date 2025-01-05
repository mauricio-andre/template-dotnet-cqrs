using System.Diagnostics;
using CqrsProject.App.DbMigrator.Interfaces;
using CqrsProject.App.DbMigrator.Loggers;
using CqrsProject.App.DbMigrator.Services;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Core.Tenants.Services;
using CqrsProject.CustomConsoleFormatter.Extensions;
using CqrsProject.OpenTelemetry.Extensions;
using CqrsProject.Postegre.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    .AddSingleton(_ => new ActivitySource(builder.Configuration.GetValue<string>("ServiceName")!));

// configuration identity
builder.Services
    .AddIdentityCore<User>()
    .AddEntityFrameworkStores<AdministrationDbContext>();

// Configure providers
builder.Services.AddCustomConsoleFormatterProvider<LoggerPropertiesService>();
builder.AddOpenTelemetryProvider();

// Configure additional appsettings
builder.Configuration
    .AddJsonFile("appsettings.Development.json", true)
    .AddEnvironmentVariables();

var app = builder.Build();

await app.StartAsync();

var activitySource = app.Services.GetRequiredService<ActivitySource>();
using (var activity = activitySource.StartActivity("RunDbMigrator"))
    await app.Services
        .GetRequiredService<IDbMigratorService>()
        .RunMigrateAsync();
