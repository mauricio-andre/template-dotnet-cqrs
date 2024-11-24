namespace CqrsProject.Common.Providers.KeyVaults;

public interface IKeyVaultService
{
    Task<T> GetKeyValueAsync<T>(string keyName);
}
