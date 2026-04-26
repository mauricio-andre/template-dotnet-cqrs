using CqrsProject.Common.Consts;
using Microsoft.AspNetCore.Authorization;
using CqrsProject.Core.Identity.Consts;

namespace CqrsProject.App.RestServer.Authorization;

public static class AuthorizationPolicyFactory
{
    public static Action<AuthorizationOptions> CreateDefaultPolicies()
    {
        return options =>
        {
            options.AddPolicy(
                AuthorizationPolicyNames.CanManageAdministration,
                policy => policy.RequireClaim(
                    IdentityPermissionClaimDefaults.ClaimType,
                    IdentityPermissionClaimDefaults.ManageAdministration));

            options.AddPolicy(
                AuthorizationPolicyNames.CanReadExamples,
                policy => policy.RequireClaim(
                    IdentityPermissionClaimDefaults.ClaimType,
                    IdentityPermissionClaimDefaults.ReadExamples));

            options.AddPolicy(
                AuthorizationPolicyNames.CanManageExamples,
                policy => policy.RequireClaim(
                    IdentityPermissionClaimDefaults.ClaimType,
                    IdentityPermissionClaimDefaults.ManageExamples));
        };
    }
}
