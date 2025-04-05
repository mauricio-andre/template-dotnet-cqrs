using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Events;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class CreateUserRoleHandler : IRequestHandler<CreateUserRoleCommand>
{
    private readonly IValidator<CreateUserRoleCommand> _validator;
    private readonly IMediator _mediator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public CreateUserRoleHandler(
        IValidator<CreateUserRoleCommand> validator,
        IMediator mediator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _validator = validator;
        _mediator = mediator;
        _stringLocalizer = stringLocalizer;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task Handle(
        CreateUserRoleCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new CreateUserRoleEvent(request.UserId, request.RoleId));

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

        await _userManager.AddToRoleAsync(user, role.NormalizedName!);
    }
}
