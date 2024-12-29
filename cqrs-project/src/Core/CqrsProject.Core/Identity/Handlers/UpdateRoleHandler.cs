using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Responses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, RoleResponse>
{
    private readonly IValidator<UpdateRoleCommand> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public UpdateRoleHandler(
        IValidator<UpdateRoleCommand> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _validator = validator;
        _stringLocalizer = stringLocalizer;
        _roleManager = roleManager;
    }

    public async Task<RoleResponse> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _roleManager.FindByIdAsync(request.Id.ToString());

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(IdentityRole<Guid>), request.Id.ToString());

        entity.Name = request.Name;
        entity.NormalizedName = _roleManager.NormalizeKey(request.Name);

        await _roleManager.UpdateAsync(entity);

        return MapToResponse(entity);
    }

    private static RoleResponse MapToResponse(IdentityRole<Guid> entity)
        => new RoleResponse(
            entity.Id,
            entity.Name!
        );
}
