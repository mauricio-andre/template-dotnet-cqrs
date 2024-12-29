using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Core.Identity.Commands;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Identity.Handlers;

public class RemoveRoleHandler : IRequestHandler<RemoveRoleCommand>
{
    private readonly IValidator<RemoveRoleCommand> _validator;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public RemoveRoleHandler(
        IValidator<RemoveRoleCommand> validator,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _validator = validator;
        _stringLocalizer = stringLocalizer;
        _roleManager = roleManager;
    }

    public async Task Handle(
        RemoveRoleCommand request,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var entity = await _roleManager.FindByIdAsync(request.Id.ToString());

        if (entity == null)
            throw new EntityNotFoundException(_stringLocalizer, nameof(IdentityRole<Guid>), request.Id.ToString());

        await _roleManager.DeleteAsync(entity);
    }
}
