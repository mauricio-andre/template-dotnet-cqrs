using FluentValidation;

namespace CqrsProject.Core.Identity.UseCases.RemoveRole;

public class RemoveRoleValidator : AbstractValidator<RemoveRoleCommand>
{
    public RemoveRoleValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
