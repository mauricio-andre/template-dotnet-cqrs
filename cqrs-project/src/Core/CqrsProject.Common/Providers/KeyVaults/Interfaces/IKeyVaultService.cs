namespace CqrsProject.Common.Providers.KeyVaults.Interfaces;

public interface IKeyVaultService
{
    ValueTask<string> GetKeyValueAsync(string keyName);
}
