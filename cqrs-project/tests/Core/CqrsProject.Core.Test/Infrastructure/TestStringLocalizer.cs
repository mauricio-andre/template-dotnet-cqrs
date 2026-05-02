using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Test.Infrastructure;

internal sealed class TestStringLocalizer<TResource> : IStringLocalizer<TResource>
{
    private readonly ConcurrentDictionary<string, string> _values = new();

    public void Set(string key, string value) => _values[key] = value;

    public LocalizedString this[string name]
        => TryGetValue(name, out var value)
            ? new LocalizedString(name, value, false)
            : new LocalizedString(name, name, true);

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var localizedString = this[name];
            if (localizedString.ResourceNotFound)
                return localizedString;

            return new LocalizedString(
                name,
                string.Format(CultureInfo.CurrentCulture, localizedString.Value, arguments),
                false);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        => _values.Select(item => new LocalizedString(item.Key, item.Value, false));

    public IStringLocalizer WithCulture(CultureInfo culture) => this;

    private bool TryGetValue(string name, out string value) => _values.TryGetValue(name, out value!);
}
