using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Rules;

public class ShallNotAllowDuplicateRoleClaimRule
    : INotificationHandler<CreateRoleClaimEvent>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public ShallNotAllowDuplicateRoleClaimRule(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _stringLocalizer = stringLocalizer;
        _roleManager = roleManager;
    }

    public async Task Handle(CreateRoleClaimEvent notification, CancellationToken cancellationToken)
    {
        var hasRole = await _administrationDbContext.RoleClaims
            .AnyAsync(
                entity => entity.RoleId == notification.RoleId
                    && entity.ClaimType == notification.ClaimType
                    && entity.ClaimValue == notification.ClaimValue,
                cancellationToken);

        if (hasRole)
            throw new DuplicatedEntityException(
                _stringLocalizer,
                nameof(IdentityRoleClaim<Guid>),
                new Dictionary<string, string[]>
                {
                    {
                        "UniqueIndexViolated",
                        [
                            _stringLocalizer[
                                "message:validation:combinationValuesAlreadyUse",
                                string.Concat(
                                    "[",
                                    notification.ClaimType,
                                    "], [",
                                    notification.ClaimValue,
                                    "]")
                            ]
                        ]
                    }
                });
    }
}
