using CqrsProject.Core.Identity.Consts;
using Microsoft.AspNetCore.Authorization;

namespace CqrsProject.App.GrpcServer.Authorization;

public static class AuthorizationPolicyFactory
{
    public static Action<AuthorizationOptions> CreateDefaultPolicies()
    {
        return options =>
        {
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
