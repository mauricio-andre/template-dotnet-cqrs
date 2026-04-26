using FluentValidation;

namespace CqrsProject.Core.Identity.UseCases.RemoveUserRole;

public class RemoveUserRoleValidator : AbstractValidator<RemoveUserRoleCommand>
{
    public RemoveUserRoleValidator()
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
