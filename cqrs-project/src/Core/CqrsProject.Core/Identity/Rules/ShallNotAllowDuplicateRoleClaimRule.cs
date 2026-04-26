using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CqrsProject.Core.Identity.UseCases.CreateRoleClaim;

namespace CqrsProject.Core.Identity.Rules;

public class ShallNotAllowDuplicateRoleClaimRule
    : INotificationHandler<CreateRoleClaimEvent>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public ShallNotAllowDuplicateRoleClaimRule(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _dbContextFactory = dbContextFactory;
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(CreateRoleClaimEvent notification, CancellationToken cancellationToken)
    {
        var administrationDbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var hasRole = await administrationDbContext.RoleClaims
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
