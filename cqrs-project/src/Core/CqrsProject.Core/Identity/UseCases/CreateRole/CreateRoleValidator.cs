using FluentValidation;

namespace CqrsProject.Core.Identity.UseCases.CreateRole;

public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(prop => prop.Name)
            .NotEmpty()
            .NotNull()
            .MaximumLength(256);
    }
}
