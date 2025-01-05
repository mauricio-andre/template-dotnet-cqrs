using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Events;
using CqrsProject.Core.Tenants.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Tenants.Rules;

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
            .AnyAsync(userTenant => userTenant.TenantId == notification.TenantId
                && userTenant.UserId == notification.UserId
                && !userTenant.Tenant!.IsDeleted
                && !userTenant.User!.IsDeleted);

        if (!hasAccess)
            throw new TenantUnreleasedException(_stringLocalizer);
    }
}
