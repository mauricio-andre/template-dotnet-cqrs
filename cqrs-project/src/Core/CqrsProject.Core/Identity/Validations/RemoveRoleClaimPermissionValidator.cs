using CqrsProject.Core.Identity.Commands;
using FluentValidation;

namespace CqrsProject.Core.Identity.Validators;

public class RemoveRoleClaimPermissionValidator : AbstractValidator<RemoveRoleClaimPermissionCommand>
{
    public RemoveRoleClaimPermissionValidator()
    {
        RuleFor(prop => prop.Name)
            .NotEmpty()
            .NotNull();

        RuleFor(prop => prop.RoleId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
