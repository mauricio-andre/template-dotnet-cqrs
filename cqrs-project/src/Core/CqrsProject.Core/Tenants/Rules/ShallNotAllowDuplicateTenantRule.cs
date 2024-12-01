using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Events;
using CqrsProject.Core.Tenants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Rules;

public class ShallNotAllowDuplicateTenantRule : INotificationHandler<CreateTenantEvent>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public ShallNotAllowDuplicateTenantRule(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(
        CreateTenantEvent notification,
        CancellationToken cancellationToken)
    {
        var hasDuplicate = await _administrationDbContext.Tenants
            .AnyAsync(tenant => tenant.Name.Equals(notification.Name));

        if (hasDuplicate)
            throw new DuplicatedEntityException(_stringLocalizer, nameof(Tenant));
    }
}
