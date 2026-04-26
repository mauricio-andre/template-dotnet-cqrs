using FluentValidation;

namespace CqrsProject.Core.Identity.UseCases.CreateRoleClaim;

public class CreateRoleClaimValidator : AbstractValidator<CreateRoleClaimCommand>
{
    public CreateRoleClaimValidator()
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
