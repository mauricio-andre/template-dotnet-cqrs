using CqrsProject.Core.Commands;
using CqrsProject.Core.Examples;
using FluentValidation;

namespace CqrsProject.Core.Validators;

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
