using CqrsProject.Core.Identity.Consts;
using CqrsProject.Core.Identity.Rules;
using CqrsProject.Core.Identity.UseCases.CreateRoleClaim;
using CqrsProject.Core.Identity.UseCases.CreateUserRole;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.UserTenants.UseCases.CreateUserTenant;
using NSubstitute;

namespace CqrsProject.Core.Test.Identity.Rules;

public class ShallNotAllowUnauthorizedUserManageSelfRuleTest
{
    [Theory(DisplayName = "Should reject self-management when the permission is missing")]
    [MemberData(nameof(GetUnauthorizedSelfCases))]
    public async Task GivenSelfTargetWithoutPermission_WhenHandled_ThenThrowUnauthorizedAccessException(
        Action<CoreTestContext> configure,
        Func<ShallNotAllowUnauthorizedUserManageSelfRule, Task> act)
    {
        using CoreTestContext context = new CoreTestContext();
        configure(context);

        ShallNotAllowUnauthorizedUserManageSelfRule rule = new ShallNotAllowUnauthorizedUserManageSelfRule(context.CurrentIdentity);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => act(rule));
    }

    [Theory(DisplayName = "Should allow self-management when the permission exists")]
    [MemberData(nameof(GetAuthorizedSelfCases))]
    public async Task GivenSelfTargetWithPermission_WhenHandled_ThenCompleteWithoutException(
        Action<CoreTestContext> configure,
        Func<ShallNotAllowUnauthorizedUserManageSelfRule, Task> act)
    {
        using CoreTestContext context = new CoreTestContext();
        configure(context);

        ShallNotAllowUnauthorizedUserManageSelfRule rule = new ShallNotAllowUnauthorizedUserManageSelfRule(context.CurrentIdentity);

        await act(rule);

        context.CurrentIdentity.Received(1).GetLocalIdentityId();
        context.CurrentIdentity.Received(1).HasLocalPermission(IdentityPermissionClaimDefaults.ManageSelf);
    }

    public static TheoryData<Action<CoreTestContext>, Func<ShallNotAllowUnauthorizedUserManageSelfRule, Task>> GetUnauthorizedSelfCases => BuildUnauthorizedSelfCases();

    public static TheoryData<Action<CoreTestContext>, Func<ShallNotAllowUnauthorizedUserManageSelfRule, Task>> GetAuthorizedSelfCases => BuildAuthorizedSelfCases();

    private static TheoryData<Action<CoreTestContext>, Func<ShallNotAllowUnauthorizedUserManageSelfRule, Task>> BuildUnauthorizedSelfCases()
    {
        TheoryData<Action<CoreTestContext>, Func<ShallNotAllowUnauthorizedUserManageSelfRule, Task>> data = new();

        {
            Guid userId = Guid.NewGuid();
            data.Add(
            context =>
            {
                context.CurrentIdentity.GetLocalIdentityId().Returns(userId);
                context.CurrentIdentity.HasLocalPermission(IdentityPermissionClaimDefaults.ManageSelf).Returns(false);
            },
            rule =>
            {
                return rule.Handle(new CreateUserTenantEvent(userId, Guid.NewGuid()), CancellationToken.None);
            }
            );
        }

        {
            Guid userId = Guid.NewGuid();
            data.Add(
            context =>
            {
                context.CurrentIdentity.GetLocalIdentityId().Returns(userId);
                context.CurrentIdentity.HasLocalPermission(IdentityPermissionClaimDefaults.ManageSelf).Returns(false);
            },
            rule =>
            {
                return rule.Handle(new CreateUserRoleEvent(userId, Guid.NewGuid()), CancellationToken.None);
            }
            );
        }

        {
            Guid userId = Guid.NewGuid();
            data.Add(
            context =>
            {
                context.CurrentIdentity.GetLocalIdentityId().Returns(userId);
                context.CurrentIdentity.HasLocalPermission(IdentityPermissionClaimDefaults.ManageSelf).Returns(false);
            },
            rule =>
            {
                return rule.Handle(new CreateRoleClaimForYourselfEvent(userId), CancellationToken.None);
            }
            );
        }

        return data;
    }

    private static TheoryData<Action<CoreTestContext>, Func<ShallNotAllowUnauthorizedUserManageSelfRule, Task>> BuildAuthorizedSelfCases()
    {
        TheoryData<Action<CoreTestContext>, Func<ShallNotAllowUnauthorizedUserManageSelfRule, Task>> data = new();

        {
            Guid userId = Guid.NewGuid();
            data.Add(
            context =>
            {
                context.CurrentIdentity.GetLocalIdentityId().Returns(userId);
                context.CurrentIdentity.HasLocalPermission(IdentityPermissionClaimDefaults.ManageSelf).Returns(true);
            },
            rule =>
            {
                return rule.Handle(new CreateUserTenantEvent(userId, Guid.NewGuid()), CancellationToken.None);
            }
            );
        }

        {
            Guid userId = Guid.NewGuid();
            data.Add(
            context =>
            {
                context.CurrentIdentity.GetLocalIdentityId().Returns(userId);
                context.CurrentIdentity.HasLocalPermission(IdentityPermissionClaimDefaults.ManageSelf).Returns(true);
            },
            rule =>
            {
                return rule.Handle(new CreateUserRoleEvent(userId, Guid.NewGuid()), CancellationToken.None);
            }
            );
        }

        {
            Guid userId = Guid.NewGuid();
            data.Add(
            context =>
            {
                context.CurrentIdentity.GetLocalIdentityId().Returns(userId);
                context.CurrentIdentity.HasLocalPermission(IdentityPermissionClaimDefaults.ManageSelf).Returns(true);
            },
            rule =>
            {
                return rule.Handle(new CreateRoleClaimForYourselfEvent(userId), CancellationToken.None);
            }
            );
        }

        return data;
    }
}
