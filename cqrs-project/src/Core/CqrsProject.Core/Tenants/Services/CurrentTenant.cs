using System.Diagnostics;
using CqrsProject.Common.Loggers;
using CqrsProject.Core.Tenants.Interfaces;
using Microsoft.Extensions.Logging;

namespace CqrsProject.Core.Tenants.Services;

public sealed class CurrentTenant : ICurrentTenant
{
    private readonly ILogger<CurrentTenant> _logger;
    private readonly Stack<Guid?> _tenantStack = new();
    private IDisposable? _loggerScope;

    public CurrentTenant(ILogger<CurrentTenant> logger)
    {
        _logger = logger;
    }

    public IDisposable BeginTenantScope(Guid? tenantId)
    {
        var previousTenant = GetCurrentTenantId();
        _tenantStack.Push(tenantId);

        if (previousTenant != null)
            _loggerScope?.Dispose();

        Activity.Current?.AddTag("tenantId", tenantId?.ToString());
        Activity.Current?.AddEvent(new ActivityEvent($"Change current tenant to \"{tenantId?.ToString()}\""));

        _loggerScope = _logger.BeginScope(new TenantLoggerRecord(tenantId));

        return new TenantScope(this, previousTenant);
    }

    public Guid? GetCurrentTenantId() => _tenantStack.Count > 0 ? _tenantStack.Peek() : null;

    public bool IsHost() => GetCurrentTenantId() == null;

    private void RestorePreviousTenant(Guid? previousTenant)
    {
        _tenantStack.Pop();

        Activity.Current?.AddEvent(new ActivityEvent($"Change current tenant to \"{previousTenant?.ToString()}\""));

        if (previousTenant != null && previousTenant != Guid.Empty)
            _loggerScope = _logger.BeginScope(new TenantLoggerRecord(previousTenant));
    }

    private sealed class TenantScope : IDisposable
    {
        private readonly CurrentTenant _currentTenant;
        private readonly Guid? _previousTenant;

        public TenantScope(CurrentTenant currentTenant, Guid? previousTenant)
        {
            _currentTenant = currentTenant;
            _previousTenant = previousTenant;
        }

        public void Dispose()
        {
            _currentTenant._loggerScope?.Dispose();
            _currentTenant.RestorePreviousTenant(_previousTenant);
        }
    }
}
