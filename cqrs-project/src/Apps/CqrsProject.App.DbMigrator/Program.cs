using CqrsProject.App.DbMigrator;
using CqrsProject.App.DbMigrator.Loggers;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity;
using CqrsProject.Core.Tenants;
using CqrsProject.CustomConsoleFormatter.Extensions;
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
    .AddPostegreCoreDbContext(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("CoreDbContext"));
    })
    .AddScoped<ITenantConnectionProvider, TenantConnectionProvider>()
    .AddScoped<ICurrentTenant, CurrentTenant>()
    .AddScoped<IDbMigratorService, DbMigratorService>();

// configuration identity
builder.Services
    .AddIdentityCore<User>()
    .AddEntityFrameworkStores<AdministrationDbContext>();

// Configure providers
builder.Services.AddCustomConsoleFormatterProvider<LoggerPropertiesService>();

var app = builder.Build();

await app.Services
    .GetRequiredService<IDbMigratorService>()
    .RunMigrateAsync();
