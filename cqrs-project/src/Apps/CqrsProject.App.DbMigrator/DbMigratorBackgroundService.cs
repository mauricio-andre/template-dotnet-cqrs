using System.Diagnostics;
using CqrsProject.App.DbMigrator.Interfaces;
using CqrsProject.Common.Diagnostics;

namespace CqrsProject.App.DbMigrator;

public sealed class DbMigratorBackgroundService : BackgroundService
{    private readonly CqrsProjectActivitySource _cqrsProjectActivitySource;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<DbMigratorBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public DbMigratorBackgroundService(
        CqrsProjectActivitySource cqrsProjectActivitySource,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<DbMigratorBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _cqrsProjectActivitySource = cqrsProjectActivitySource;
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start DbMigrator BackgroundService");

        using (_cqrsProjectActivitySource.ActivitySourceDefault.StartActivity("RunDbMigrator"))
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbMigratorService = scope.ServiceProvider.GetRequiredService<IDbMigratorService>();
            await dbMigratorService.RunMigrateAsync(stoppingToken);
        }

        _hostApplicationLifetime.StopApplication();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stop DbMigrator BackgroundService");
        return Task.CompletedTask;
    }
}
