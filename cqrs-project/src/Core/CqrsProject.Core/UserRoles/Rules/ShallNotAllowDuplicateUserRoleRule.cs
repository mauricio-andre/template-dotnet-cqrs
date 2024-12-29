using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.UserRoles.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.UserRoles.Rules;

public class ShallNotAllowDuplicateUserRoleRule: INotificationHandler<CreateUserRoleEvent>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public ShallNotAllowDuplicateUserRoleRule(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(CreateUserRoleEvent notification, CancellationToken cancellationToken)
    {
        var hasDuplicate = await _administrationDbContext.UserRoles
            .AnyAsync(
                userRole => userRole.UserId == notification.UserId
                    && userRole.RoleId == notification.RoleId,
                cancellationToken);

        if (hasDuplicate)
            throw new DuplicatedEntityException(_stringLocalizer, nameof(IdentityUserRole<Guid>));
    }
}
