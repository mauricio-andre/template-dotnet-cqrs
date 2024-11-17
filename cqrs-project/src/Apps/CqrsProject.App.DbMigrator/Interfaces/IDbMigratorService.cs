namespace CqrsProject.App.DbMigrator;

public interface IDbMigratorService
{
    Task RunMigrateAsync();
}
