using FluentValidation;

namespace CqrsProject.Core.Identity.UseCases.CreateUserRole;

public class CreateUserRoleValidator : AbstractValidator<CreateUserRoleCommand>
{
    public CreateUserRoleValidator()
    {
        RuleFor(prop => prop.UserId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(prop => prop.RoleId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
