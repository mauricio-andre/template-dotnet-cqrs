using FluentValidation;

namespace CqrsProject.Core.Identity.UseCases.RemoveRoleClaim;

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
