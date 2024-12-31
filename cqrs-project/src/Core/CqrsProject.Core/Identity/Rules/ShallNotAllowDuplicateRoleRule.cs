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

public class ShallNotAllowDuplicateRoleRule
    : INotificationHandler<CreateRoleEvent>,
    INotificationHandler<UpdateRoleEvent>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public ShallNotAllowDuplicateRoleRule(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _stringLocalizer = stringLocalizer;
        _roleManager = roleManager;
    }

    public Task Handle(CreateRoleEvent notification, CancellationToken cancellationToken)
        => HandleRule(null, notification.Name, cancellationToken);

    public Task Handle(UpdateRoleEvent notification, CancellationToken cancellationToken)
        => HandleRule(notification.Id, notification.Name, cancellationToken);

    private async Task HandleRule(Guid? id, string name, CancellationToken cancellationToken)
    {
        var hasRole = await _administrationDbContext.Roles
            .AnyAsync(
                entity => entity.Id != id
                    && entity.NormalizedName == _roleManager.NormalizeKey(name),
                cancellationToken);

        if (hasRole)
            throw new DuplicatedEntityException(
                _stringLocalizer,
                nameof(User),
                new Dictionary<string, string[]>
                {
                    { nameof(IdentityRole.Name), [ _stringLocalizer["message:validation:valueAlreadyUse", name] ] }
                });
    }
}
