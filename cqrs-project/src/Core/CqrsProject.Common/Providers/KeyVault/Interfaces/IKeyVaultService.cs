namespace CqrsProject.Common.Providers.KeyVaults;

public interface IKeyVaultService
{
    ValueTask<T> GetKeyValueAsync<T>(string keyName);
}
