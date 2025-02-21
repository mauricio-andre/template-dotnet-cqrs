using System.Collections;

namespace CqrsProject.Common.Loggers;

public record TenantLoggerRecord : IEnumerable<KeyValuePair<string, object?>>
{
    public Guid? TenantId { get; }

    public TenantLoggerRecord(Guid? tenantId = null)
    {
        TenantId = tenantId;
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        yield return new KeyValuePair<string, object?>("tenantId", TenantId?.ToString() ?? "");
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
