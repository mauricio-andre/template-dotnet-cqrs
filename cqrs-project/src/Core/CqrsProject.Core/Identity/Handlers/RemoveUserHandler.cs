using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class RemoveUserHandler : IRequestHandler<RemoveUserCommand>
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly IValidator<RemoveUserCommand> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly UserManager<User> _userManager;

    public RemoveUserHandler(
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IValidator<RemoveUserCommand> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        UserManager<User> userManager)
    {
        _administrationDbContext = dbContextFactory.CreateDbContext();
        _validator = validator;
        _stringLocalizer = stringLocalizer;
        _userManager = userManager;
    }

    public async Task Handle(
        RemoveUserCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(User), request.Id.ToString());

        user.IsDeleted = true;
        await _userManager.UpdateAsync(user);
        await RemoveRolesAsync(user);
        await RemoveClaimsAsync(user);
        await RemoveTenantsAsync(request, cancellationToken);
    }

    private async Task RemoveRolesAsync(User user)
    {
        var roleList = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, roleList);
    }

    private async Task RemoveClaimsAsync(User user)
    {
        var claimList = await _userManager.GetClaimsAsync(user);
        await _userManager.RemoveClaimsAsync(user, claimList);
    }

    private async Task RemoveTenantsAsync(RemoveUserCommand request, CancellationToken cancellationToken)
    {
        await _administrationDbContext.UserTenants
            .Where(entity => entity.UserId == request.Id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
