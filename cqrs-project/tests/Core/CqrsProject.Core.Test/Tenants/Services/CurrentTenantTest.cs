using System.Diagnostics;
using CqrsProject.Core.Tenants.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CqrsProject.Core.Test.Tenants.Services;

public class CurrentTenantTest
{
    [Fact(DisplayName = "Should start as host and expose no current tenant")]
    public void GivenNewCurrentTenant_WhenReadingState_ThenReturnHost()
    {
        ILogger<CurrentTenant> logger = Substitute.For<ILogger<CurrentTenant>>();
        CurrentTenant currentTenant = new CurrentTenant(logger);

        Assert.True(currentTenant.IsHost());
        Assert.Null(currentTenant.GetCurrentTenantId());
    }

    [Fact(DisplayName = "Should set and restore nested tenant scopes")]
    public void GivenNestedScopes_WhenDisposing_ThenRestorePreviousTenant()
    {
        ILogger<CurrentTenant> logger = Substitute.For<ILogger<CurrentTenant>>();
        CurrentTenant currentTenant = new CurrentTenant(logger);
        Guid tenantOneId = Guid.NewGuid();
        Guid tenantTwoId = Guid.NewGuid();

        using (currentTenant.BeginTenantScope(tenantOneId))
        {
            Assert.False(currentTenant.IsHost());
            Assert.Equal(tenantOneId, currentTenant.GetCurrentTenantId());

            using (currentTenant.BeginTenantScope(tenantTwoId))
            {
                Assert.Equal(tenantTwoId, currentTenant.GetCurrentTenantId());
            }

            Assert.Equal(tenantOneId, currentTenant.GetCurrentTenantId());
        }

        Assert.True(currentTenant.IsHost());
        Assert.Null(currentTenant.GetCurrentTenantId());
    }

    [Fact(DisplayName = "Should allow switching back to host tenant scope")]
    public void GivenHostScope_WhenDisposing_ThenReturnToHost()
    {
        ILogger<CurrentTenant> logger = Substitute.For<ILogger<CurrentTenant>>();
        CurrentTenant currentTenant = new CurrentTenant(logger);
        Guid tenantId = Guid.NewGuid();

        using (currentTenant.BeginTenantScope(tenantId))
        {
            Assert.Equal(tenantId, currentTenant.GetCurrentTenantId());
        }

        Assert.True(currentTenant.IsHost());
        Assert.Null(currentTenant.GetCurrentTenantId());
    }
}

