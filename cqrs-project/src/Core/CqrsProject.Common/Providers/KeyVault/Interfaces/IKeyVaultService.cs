namespace CqrsProject.Common.Providers.KeyVaults;

public interface IKeyVaultService
{
    ValueTask<string> GetKeyValueAsync(string keyName);
}
