using System.Text.Json;
using CqrsProject.Common.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace CqrsProject.CustomStringLocalizer.Services;

public class StringLocalizerService : IStringLocalizer<CqrsProjectResource>
{
    private readonly IMemoryCache _memoryCache;

    public StringLocalizerService(IMemoryCache memoryCache) => _memoryCache = memoryCache;

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var actualValue = this[name];
            return !actualValue.ResourceNotFound
                ? new LocalizedString(
                    name,
                    string.Format(actualValue.Value, arguments),
                    false)
                : actualValue;
        }
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, string.IsNullOrEmpty(value));
        }
    }

    private string? GetString(string key)
    {
        string filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Jsons",
            string.Concat(Thread.CurrentThread.CurrentCulture.Name, ".json"));

        if (File.Exists(filePath))
            return _memoryCache.GetOrCreate(
                $"location_{Thread.CurrentThread.CurrentCulture.Name}_{key}",
                cache => GetValueFromJson(key, filePath)
            );

        return default;
    }

    private static string? GetValueFromJson(string key, string filePath)
    {
        using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var streamReader = new StreamReader(str))
        {
            var jsonDocument = JsonDocument.Parse(streamReader.BaseStream);
            if (jsonDocument.RootElement.TryGetProperty(key, out var jsonProperty))
                return jsonProperty.GetString()!;
        }

        return default;
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
    {
        string filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Jsons",
            string.Concat(Thread.CurrentThread.CurrentCulture.Name, ".json"));

        using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (var streamReader = new StreamReader(str))
        {
            var jsonDocument = JsonDocument.Parse(streamReader.BaseStream);
            foreach (var property in jsonDocument.RootElement.EnumerateObject())
                yield return new LocalizedString(property.Name, property.Value.ToString()!, false);
        }
    }
}
