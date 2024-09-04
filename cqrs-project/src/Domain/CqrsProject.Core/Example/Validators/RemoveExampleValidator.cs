using CqrsProject.Core.Commands;
using FluentValidation;

namespace CqrsProject.Core.Validators;

public class RemoveExampleValidator : AbstractValidator<RemoveExampleCommand>
{
    public RemoveExampleValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull();
    }
}
