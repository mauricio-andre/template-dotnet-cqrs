using CqrsProject.Core.Identity.Commands;
using FluentValidation;

namespace CqrsProject.Core.Identity.Validators;

public class RemoveRoleClaimValidator : AbstractValidator<RemoveRoleClaimCommand>
{
    public RemoveRoleClaimValidator()
    {
        RuleFor(prop => prop.ClaimType)
            .NotEmpty()
            .NotNull();

        RuleFor(prop => prop.ClaimValue)
            .NotEmpty()
            .NotNull();

        RuleFor(prop => prop.RoleId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
