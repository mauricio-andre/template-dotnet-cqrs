using Testcontainers.PostgreSql;

namespace CqrsProject.App.DbMigratorTest.Database;

public class PostgresDocker : IDatabaseDocker
{
    private readonly PostgreSqlContainer _postgresContainer;
    private const string AdministrationSchemaName = "administration";
    private const string CoreSchemaName = "host";

    public PostgresDocker()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();

        _postgresContainer.StartAsync().GetAwaiter().GetResult();

        CreateSchemaIfNotExists(AdministrationSchemaName);
        CreateSchemaIfNotExists(CoreSchemaName);
    }

    public void CreateSchemaIfNotExists(string schema)
    {
        _postgresContainer.ExecScriptAsync($"CREATE SCHEMA IF NOT EXISTS {schema};");
    }

    public string GetConnectionStringSearchPath(string schema)
    {
        var connectionString = _postgresContainer.GetConnectionString();
        return string.Concat(connectionString, ";SearchPath=", schema);
    }

    public string GetConnectionStringAdministration()
    {
        return GetConnectionStringSearchPath(AdministrationSchemaName);
    }

    public string GetConnectionStringHost()
    {
        return GetConnectionStringSearchPath(CoreSchemaName);
    }
}
