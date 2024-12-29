using CqrsProject.Core.Identity.Commands;
using FluentValidation;

namespace CqrsProject.Core.Tenants.Validators;

public class CreateRoleClaimPermissionValidator : AbstractValidator<CreateRoleClaimPermissionCommand>
{
    public CreateRoleClaimPermissionValidator()
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
