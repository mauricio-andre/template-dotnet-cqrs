using CqrsProject.Common.Consts;
using Microsoft.AspNetCore.Authorization;

namespace CqrsProject.App.RestServer.Authorization;

public static class AuthorizationPolicyFactory
{
    public static Action<AuthorizationOptions> CreateDefaultPolicies()
    {
        return options =>
        {
            options.AddPolicy(
                AuthorizationPolicyNames.CanReadExamples,
                policy => policy.RequireClaim(AuthorizationPermissionClaims.ClaimType, AuthorizationPermissionClaims.ReadExamples));

            options.AddPolicy(
                AuthorizationPolicyNames.CanManageExamples,
                policy => policy.RequireClaim(AuthorizationPermissionClaims.ClaimType, AuthorizationPermissionClaims.ManageExamples));

            options.AddPolicy(
                AuthorizationPolicyNames.CanManageSelf,
                policy => policy.RequireClaim(AuthorizationPermissionClaims.ClaimType, AuthorizationPermissionClaims.ManageSelf));
        };
    }
}
