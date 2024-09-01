using CqrsProject.App.DbMigrator;
using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.Identity.Services;
using CqrsProject.Core.Tenants;
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
    .AddSingleton<ITenantConnectionProvider, TenantConnectionProvider>()
    .AddScoped<IIdentitySyncService, IdentitySyncService>()
    .AddScoped<ICurrentTenant, CurrentTenant>()
    .AddScoped<IDbMigratorService, DbMigratorService>();

var app = builder.Build();

await app.Services
    .GetRequiredService<IDbMigratorService>()
    .RunMigrateAsync();
