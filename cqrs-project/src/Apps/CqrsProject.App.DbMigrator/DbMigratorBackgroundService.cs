using System.Diagnostics;
using CqrsProject.App.DbMigrator.Interfaces;
using CqrsProject.Common.Diagnostics;

namespace CqrsProject.App.DbMigrator;

public sealed class DbMigratorBackgroundService : BackgroundService
{
    private readonly IDbMigratorService _dbMigratorService;
    private readonly CqrsProjectActivitySource _cqrsProjectActivitySource;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<DbMigratorBackgroundService> _logger;

    public DbMigratorBackgroundService(
        IDbMigratorService dbMigratorService,
        CqrsProjectActivitySource cqrsProjectActivitySource,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<DbMigratorBackgroundService> logger)
    {
        _dbMigratorService = dbMigratorService;
        _cqrsProjectActivitySource = cqrsProjectActivitySource;
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start DbMigrator BackgroundService");

        using (_cqrsProjectActivitySource.ActivitySourceDefault.StartActivity("RunDbMigrator"))
            await _dbMigratorService.RunMigrateAsync(stoppingToken);

        _hostApplicationLifetime.StopApplication();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stop DbMigrator BackgroundService");
        return Task.CompletedTask;
    }
}
