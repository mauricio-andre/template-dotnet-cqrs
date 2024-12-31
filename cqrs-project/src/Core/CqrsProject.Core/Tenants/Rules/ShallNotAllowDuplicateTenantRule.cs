using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Tenants.Rules;

public class ShallNotAllowDuplicateTenantRule
    : INotificationHandler<CreateTenantEvent>,
    INotificationHandler<UpdateTenantEvent>
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

    public Task Handle(CreateTenantEvent notification, CancellationToken cancellationToken)
        => HandleRule(null, notification.Name, cancellationToken);

    public Task Handle(UpdateTenantEvent notification, CancellationToken cancellationToken)
        => HandleRule(notification.Id, notification.Name, cancellationToken);

    private async Task HandleRule(Guid? id, string name, CancellationToken cancellationToken)
    {
        var hasDuplicate = await _administrationDbContext.Tenants
            .AnyAsync(
                tenant => tenant.Id != id && tenant.Name.ToLower() == name.ToLower(),
                cancellationToken);

        if (hasDuplicate)
            throw new DuplicatedEntityException(
                _stringLocalizer,
                nameof(Tenant),
                new Dictionary<string, string[]>
                {
                    {
                        nameof(Tenant.Name),
                        [ _stringLocalizer["message:validation:valueAlreadyUse", name] ]
                    }
                });
    }
}
