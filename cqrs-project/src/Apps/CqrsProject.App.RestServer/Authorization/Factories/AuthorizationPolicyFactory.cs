using CqrsProject.Core.Identity.Consts;
using Microsoft.AspNetCore.Authorization;

namespace CqrsProject.App.RestServer.Authorization;

public static class AuthorizationPolicyFactory
{
    public static Action<AuthorizationOptions> CreateDefaultPolicies()
    {
        return options =>
        {
            options.AddPolicy(
                AuthorizationPolicyNames.HasRoleHostAdmin,
                policy => policy.RequireRole(IdentityRoleDefaults.HostAdmin));

            options.AddPolicy(
                AuthorizationPolicyNames.HasRoleTenantAdmin,
                policy => policy.RequireRole(IdentityRoleDefaults.TenantAdmin));

            options.AddPolicy(
                AuthorizationPolicyNames.HasRoleClient,
                policy => policy.RequireRole(IdentityRoleDefaults.Client));
        };
    }
}
