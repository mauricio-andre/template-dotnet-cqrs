using FluentValidation;
using MediatR;
using CqrsProject.Core.Identity.Commands;
using Microsoft.AspNetCore.Identity;
using CqrsProject.Common.Consts;
using System.Reflection;
using CqrsProject.Common.Exceptions;
using Microsoft.Extensions.Localization;
using CqrsProject.Common.Localization;
using System.Security.Claims;

namespace CqrsProject.Core.Identity.Handlers;

public class CreateRoleClaimPermissionHandler : IRequestHandler<CreateRoleClaimPermissionCommand>
{
    private readonly IValidator<CreateRoleClaimPermissionCommand> _validator;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public CreateRoleClaimPermissionHandler(
        IValidator<CreateRoleClaimPermissionCommand> validator,
        RoleManager<IdentityRole<Guid>> roleManager,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _validator = validator;
        _roleManager = roleManager;
        _stringLocalizer = stringLocalizer;
    }

    public async Task Handle(
        CreateRoleClaimPermissionCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
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
