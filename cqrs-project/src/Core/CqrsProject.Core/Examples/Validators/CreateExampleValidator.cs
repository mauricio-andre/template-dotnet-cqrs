using CqrsProject.Core.Examples.Commands;
using CqrsProject.Core.Examples.Entities;
using FluentValidation;

namespace CqrsProject.Core.Examples.Validators;

public class CreateExampleValidator : AbstractValidator<CreateExampleCommand>
{
    public CreateExampleValidator()
    {
        RuleFor(prop => prop.Name)
            .NotEmpty()
            .NotNull()
            .MaximumLength(ExampleConstrains.NameMaxLength);
    }
}
