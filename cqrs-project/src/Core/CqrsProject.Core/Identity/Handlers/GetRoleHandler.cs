using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Identity.Queries;
using CqrsProject.Core.Identity.Responses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class GetRoleHandler : IRequestHandler<GetRoleQuery, RoleResponse>
{
    private readonly IValidator<GetRoleQuery> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public GetRoleHandler(
        IValidator<GetRoleQuery> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _validator = validator;
        _stringLocalizer = stringLocalizer;
        _roleManager = roleManager;
    }

    public async Task<RoleResponse> Handle(
        GetRoleQuery request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _roleManager.FindByIdAsync(request.Id.ToString());

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(IdentityRole<Guid>), request.Id.ToString());

        return MapToResponse(entity);
    }

    private static RoleResponse MapToResponse(IdentityRole<Guid> entity)
        => new RoleResponse(
            entity.Id,
            entity.Name!);
}
