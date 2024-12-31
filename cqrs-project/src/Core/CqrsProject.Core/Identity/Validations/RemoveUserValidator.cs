using CqrsProject.Core.Identity.Commands;
using FluentValidation;

namespace CqrsProject.Core.Identity.Validators;

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
