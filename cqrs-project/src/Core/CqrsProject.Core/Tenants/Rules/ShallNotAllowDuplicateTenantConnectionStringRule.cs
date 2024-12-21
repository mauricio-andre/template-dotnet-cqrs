using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Rules;

public class ShallNotAllowDuplicateTenantConnectionStringRule
    : INotificationHandler<CreateTenantConnectionStringEvent>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public ShallNotAllowDuplicateTenantConnectionStringRule(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(CreateTenantConnectionStringEvent notification, CancellationToken cancellationToken)
    {
        var hasDuplicate = await _administrationDbContext.TenantConnectionStrings
            .AnyAsync(
                entity => entity.TenantId != notification.TenantId
                    && entity.ConnectionName.ToLower() == notification.ConnectionName.ToLower(),
                cancellationToken);

        if (hasDuplicate)
            throw new DuplicatedEntityException(_stringLocalizer, nameof(Tenant));
    }
}
