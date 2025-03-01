using CqrsProject.Core.Identity.Commands;
using CqrsProject.Core.Identity.Events;
using CqrsProject.Core.Identity.Responses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CqrsProject.Core.Identity.Handlers;

public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, RoleResponse>
{
    private readonly IValidator<CreateRoleCommand> _validator;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IMediator _mediator;

    public CreateRoleHandler(
        IValidator<CreateRoleCommand> validator,
        RoleManager<IdentityRole<Guid>> roleManager,
        IMediator mediator)
    {
        _validator = validator;
        _roleManager = roleManager;
        _mediator = mediator;
    }

    public async Task<RoleResponse> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        await _mediator.Publish(new CreateRoleEvent(request.Name));

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
