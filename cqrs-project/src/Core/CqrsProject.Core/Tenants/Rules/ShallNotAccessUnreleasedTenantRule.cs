using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Events;
using CqrsProject.Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Rules;

public class ShallNotAccessUnreleasedTenantRule : INotificationHandler<TenantAccessedByUserEvent>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public ShallNotAccessUnreleasedTenantRule(
        AdministrationDbContext administrationDbContext,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _administrationDbContext = administrationDbContext;
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(TenantAccessedByUserEvent notification, CancellationToken cancellationToken)
    {
        var hasAccess = await _administrationDbContext.UserTenants
            .AnyAsync(userTenant => userTenant.TenantId == notification.TenantId &&
                userTenant.UserId == notification.TenantId &&
                !userTenant.Tenant!.IsDeleted);

        if (!hasAccess)
            throw new TenantUnreleasedException(_stringLocalizer);
    }
}
