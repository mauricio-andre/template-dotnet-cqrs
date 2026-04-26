using FluentValidation;

namespace CqrsProject.Core.Identity.UseCases.RemoveUser;

public class RemoveUserValidator : AbstractValidator<RemoveUserCommand>
{
    public RemoveUserValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
