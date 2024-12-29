using FluentValidation;
using MediatR;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.Identity.Commands;
using Microsoft.AspNetCore.Identity;

namespace CqrsProject.Core.Identity.Handlers;

public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, RoleResponse>
{
    private readonly IValidator<CreateRoleCommand> _validator;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public CreateRoleHandler(
        IValidator<CreateRoleCommand> validator,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _validator = validator;
        _roleManager = roleManager;
    }

    public async Task<RoleResponse> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = MapToEntity(request);

        await _roleManager.CreateAsync(entity);

        return MapToResponse(entity);
    }

    private IdentityRole<Guid> MapToEntity(CreateRoleCommand request)
        => new IdentityRole<Guid> { Name = request.Name, NormalizedName = _roleManager.NormalizeKey(request.Name) };

    private static RoleResponse MapToResponse(IdentityRole<Guid> entity)
        => new RoleResponse(
            entity.Id,
            entity.Name!
        );
}
