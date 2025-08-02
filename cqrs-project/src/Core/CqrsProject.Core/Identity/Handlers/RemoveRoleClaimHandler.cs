using System.Security.Claims;
using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Identity.Commands;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class RemoveRoleClaimHandler : IRequestHandler<RemoveRoleClaimCommand>
{
    private readonly IValidator<RemoveRoleClaimCommand> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RemoveRoleClaimHandler(
        IValidator<RemoveRoleClaimCommand> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _validator = validator;
        _stringLocalizer = stringLocalizer;
        _roleManager = roleManager;
    }

    public async Task Handle(
        RemoveRoleClaimCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var role = await GetRole(request);
        var claim = new Claim(request.ClaimType, request.ClaimValue);

        await _roleManager.RemoveClaimAsync(role, claim);
    }

    private async Task<IdentityRole<Guid>> GetRole(RemoveRoleClaimCommand request)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

        if (role == null)
            throw new EntityNotFoundException(
                _stringLocalizer,
                nameof(IdentityRole<Guid>),
                request.RoleId.ToString());

        return role;
    }
}
