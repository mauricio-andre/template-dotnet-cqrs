namespace CqrsProject.App.DbMigratorTest.Database;

public interface IDatabaseDocker
{
    public void CreateSchemaIfNotExists(string schema);
    public string GetConnectionStringAdministration();
    public string GetConnectionStringHost();
    public string GetConnectionStringSearchPath(string schema);
}
