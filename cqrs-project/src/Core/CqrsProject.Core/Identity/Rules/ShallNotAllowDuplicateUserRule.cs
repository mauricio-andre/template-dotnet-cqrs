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

public class ShallNotAllowDuplicateUserRule
    : INotificationHandler<CreateUserEvent>,
    INotificationHandler<UpdateUserEvent>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly UserManager<User> _userManager;

    public ShallNotAllowDuplicateUserRule(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        UserManager<User> userManager)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _stringLocalizer = stringLocalizer;
        _userManager = userManager;
    }

    public Task Handle(CreateUserEvent notification, CancellationToken cancellationToken)
        => HandleRule(null, notification.UserName, notification.Email, cancellationToken);

    public Task Handle(UpdateUserEvent notification, CancellationToken cancellationToken)
        => HandleRule(notification.Id, notification.UserName, notification.Email, cancellationToken);

    private async Task HandleRule(Guid? id, string userName, string email, CancellationToken cancellationToken)
    {
        var user = await _administrationDbContext.Users
            .FirstOrDefaultAsync(
                entity => entity.Id != id
                    && (entity.NormalizedUserName == _userManager.NormalizeName(userName)
                    || entity.NormalizedEmail == _userManager.NormalizeEmail(email)),
                cancellationToken);

        if (user != null)
        {
            var errors = new Dictionary<string, string[]>();

            if (user.NormalizedUserName == _userManager.NormalizeName(userName))
                errors.Add(nameof(User.UserName), [_stringLocalizer["message:validation:valueAlreadyUse", userName]]);

            if (user.NormalizedEmail == _userManager.NormalizeEmail(email))
                errors.Add(nameof(User.Email), [_stringLocalizer["message:validation:valueAlreadyUse", email]]);

            throw new DuplicatedEntityException(_stringLocalizer, nameof(User), errors);
        }
    }
}
