using System.Reflection;
using System.Security.Claims;
using CqrsProject.Common.Consts;
using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Events;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class CreateRoleClaimPermissionHandler : IRequestHandler<CreateRoleClaimPermissionCommand>
{
    private readonly IValidator<CreateRoleClaimPermissionCommand> _validator;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly IMediator _mediator;

    public CreateRoleClaimPermissionHandler(
        IValidator<CreateRoleClaimPermissionCommand> validator,
        RoleManager<IdentityRole<Guid>> roleManager,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        IMediator mediator)
    {
        _validator = validator;
        _roleManager = roleManager;
        _stringLocalizer = stringLocalizer;
        _mediator = mediator;
    }

    public async Task Handle(
        CreateRoleClaimPermissionCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new CreateRoleClaimEvent(
            request.RoleId,
            AuthorizationPermissionClaims.ClaimType,
            request.Name));

        var role = await GetRole(request);
        var permissionClaim = GetClaim(request);

        await _roleManager.AddClaimAsync(role, permissionClaim);
    }

    private async Task<IdentityRole<Guid>> GetRole(CreateRoleClaimPermissionCommand request)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());

        if (role == null)
            throw new EntityNotFoundException(
                _stringLocalizer,
                nameof(IdentityRole<Guid>),
                request.RoleId.ToString());

        return role;
    }

    private Claim GetClaim(CreateRoleClaimPermissionCommand request)
    {
        var permissionClaim = typeof(AuthorizationPermissionClaims)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f =>
                f.IsLiteral
                && !f.IsInitOnly
                && f.FieldType == typeof(string)
                && f.GetRawConstantValue()?.ToString()?.ToLower() == request.Name.ToLower())
            .Select(f => new Claim(AuthorizationPermissionClaims.ClaimType, f.GetRawConstantValue()!.ToString()!))
            .FirstOrDefault();

        if (permissionClaim == null)
            throw new EntityNotFoundException(
                _stringLocalizer,
                nameof(AuthorizationPermissionClaims),
                request.Name);

        return permissionClaim;
    }
}
