using CqrsProject.Core.Examples.Commands;
using FluentValidation;

namespace CqrsProject.Core.Examples.Validators;

public class RemoveExampleValidator : AbstractValidator<RemoveExampleCommand>
{
    public RemoveExampleValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull();
    }
}
