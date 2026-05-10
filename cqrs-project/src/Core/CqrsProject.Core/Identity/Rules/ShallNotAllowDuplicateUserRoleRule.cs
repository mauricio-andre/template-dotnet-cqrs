using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.UseCases.CreateUserRole;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Rules;

public class ShallNotAllowDuplicateUserRoleRule : INotificationHandler<CreateUserRoleEvent>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public ShallNotAllowDuplicateUserRoleRule(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _dbContextFactory = dbContextFactory;
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(CreateUserRoleEvent notification, CancellationToken cancellationToken)
    {
        var administrationDbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var hasDuplicate = await administrationDbContext.UserRoles
            .AnyAsync(
                userRole => userRole.UserId == notification.UserId
                    && userRole.RoleId == notification.RoleId,
                cancellationToken);

        if (hasDuplicate)
            throw new DuplicatedEntityException(_stringLocalizer, nameof(IdentityUserRole<Guid>));
    }
}
