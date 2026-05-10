using System.Security.Claims;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Identity.Consts;
using CqrsProject.Core.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CqrsProject.Core.Test.Identity.Services;

public class CurrentIdentityTest
{
    [Fact(DisplayName = "Should expose the current identity and its claims")]
    public void GivenClaimsPrincipal_WhenReadingCurrentIdentity_ThenReturnExpectedValues()
    {
        CurrentIdentity currentIdentity = new CurrentIdentity();
        ClaimsIdentity identity = new ClaimsIdentity(AuthenticationDefaults.LocalIdentityType);
        Guid userId = Guid.NewGuid();
        Guid tenantId = Guid.NewGuid();

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
        identity.AddClaim(new Claim(IdentityPermissionClaimDefaults.ClaimType, IdentityPermissionClaimDefaults.ManageSelf));
        identity.AddClaim(new Claim("tenants", tenantId.ToString()));

        currentIdentity.SetCurrentIdentity(new ClaimsPrincipal(identity));

        Assert.True(currentIdentity.HasLocalIdentity());
        Assert.Equal(userId, currentIdentity.GetLocalIdentityId());
        Assert.True(currentIdentity.HasLocalPermission(IdentityPermissionClaimDefaults.ManageSelf));
        Assert.Equal(["Admin"], currentIdentity.GetRoles());
        Assert.Equal([tenantId], currentIdentity.GetTenants());
    }

    [Fact(DisplayName = "Should fail when the current identity is missing")]
    public void GivenEmptyIdentity_WhenReadingLocalId_ThenThrowUnauthorizedAccessException()
    {
        CurrentIdentity currentIdentity = new CurrentIdentity();

        Assert.False(currentIdentity.HasLocalIdentity());
        Assert.False(currentIdentity.HasLocalPermission("any"));
        Assert.Throws<UnauthorizedAccessException>(() => currentIdentity.GetLocalIdentityId());
    }
}

