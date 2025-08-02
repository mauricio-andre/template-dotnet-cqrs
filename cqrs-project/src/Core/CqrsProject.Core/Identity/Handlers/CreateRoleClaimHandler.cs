using System.Security.Claims;
using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Events;
using CqrsProject.Core.Identity.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class CreateRoleClaimHandler : IRequestHandler<CreateRoleClaimCommand>
{
    private readonly IValidator<CreateRoleClaimCommand> _validator;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly IMediator _mediator;
    private readonly ICurrentIdentity _currentIdentity;

    public CreateRoleClaimHandler(
        IValidator<CreateRoleClaimCommand> validator,
        RoleManager<IdentityRole<Guid>> roleManager,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        IMediator mediator,
        ICurrentIdentity currentIdentity)
    {
        _validator = validator;
        _roleManager = roleManager;
        _stringLocalizer = stringLocalizer;
        _mediator = mediator;
        _currentIdentity = currentIdentity;
    }

    public async Task Handle(
        CreateRoleClaimCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new CreateRoleClaimEvent(
            request.RoleId,
            request.ClaimType,
            request.ClaimValue));

        var role = await GetRole(request);
        await IsUserInRole(role);

        var claim = new Claim(request.ClaimType, request.ClaimValue);
        await _roleManager.AddClaimAsync(role, claim);
    }
    private async Task<IdentityRole<Guid>> GetRole(CreateRoleClaimCommand request)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

        if (role == null)
            throw new EntityNotFoundException(
                _stringLocalizer,
                nameof(IdentityRole<Guid>),
                request.RoleId.ToString());

        return role;
    }

    private async Task IsUserInRole(IdentityRole<Guid> role)
    {
        var roles = _currentIdentity.GetRoles()?.Select(role => _roleManager.NormalizeKey(role));
        if (roles != null && roles.Contains(role.NormalizedName))
            await _mediator.Publish(new CreateRoleClaimForYourselfEvent(_currentIdentity.GetLocalIdentityId()));
    }

}
