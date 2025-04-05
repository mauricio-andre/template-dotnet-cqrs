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

public class RemoveUserRoleHandler : IRequestHandler<RemoveUserRoleCommand>
{
    private readonly IValidator<RemoveUserRoleCommand> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RemoveUserRoleHandler(
        IValidator<RemoveUserRoleCommand> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _validator = validator;
        _stringLocalizer = stringLocalizer;
        _userManager = userManager;
        _roleManager = roleManager;

    }

    public async Task Handle(
        RemoveUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null || user.IsDeleted)
            throw new EntityNotFoundException(
                _stringLocalizer,
                nameof(User),
                request.UserId.ToString());

        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

        if (role == null)
            throw new EntityNotFoundException(
                _stringLocalizer,
                nameof(IdentityRole<Guid>),
                request.RoleId.ToString());

        await _userManager.RemoveFromRoleAsync(user, role.NormalizedName!);
    }
}
