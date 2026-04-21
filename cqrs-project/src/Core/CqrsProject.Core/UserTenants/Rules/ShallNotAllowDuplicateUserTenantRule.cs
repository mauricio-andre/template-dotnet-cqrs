using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.UserTenants.Entities;
using CqrsProject.Core.UserTenants.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.UserTenants.Rules;

public class ShallNotAllowDuplicateUserTenantRule : INotificationHandler<CreateUserTenantEvent>
{
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public ShallNotAllowDuplicateUserTenantRule(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _dbContextFactory = dbContextFactory;
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(CreateUserTenantEvent notification, CancellationToken cancellationToken)
    {
        var administrationDbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var hasDuplicate = await administrationDbContext.UserTenants
            .AnyAsync(
                userTenant => userTenant.UserId == notification.UserId
                    && userTenant.TenantId == notification.TenantId,
                cancellationToken);

        if (hasDuplicate)
            throw new DuplicatedEntityException(_stringLocalizer, nameof(UserTenant));
    }
}
