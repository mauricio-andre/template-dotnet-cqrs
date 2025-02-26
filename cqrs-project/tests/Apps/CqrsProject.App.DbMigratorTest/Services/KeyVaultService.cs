using CqrsProject.App.DbMigratorTest.Database;
using CqrsProject.Common.Providers.KeyVaults.Interfaces;

namespace CqrsProject.App.DbMigratorTest.Services;

public class KeyVaultService : IKeyVaultService
{
    private readonly IDatabaseDocker _databaseDocker;

    public KeyVaultService(IDatabaseDocker postgresDocker)
    {
        _databaseDocker = postgresDocker;
    }

    public ValueTask<string> GetKeyValueAsync(string keyName)
    {
        _databaseDocker.CreateSchemaIfNotExists(keyName);
        return ValueTask.FromResult(_databaseDocker.GetConnectionStringSearchPath(keyName));
    }
}
