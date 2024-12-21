namespace CqrsProject.App.DbMigrator.Interfaces;

public interface IDbMigratorService
{
    Task RunMigrateAsync(CancellationToken cancellationToken = default);
}
