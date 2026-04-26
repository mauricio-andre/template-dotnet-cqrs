using CqrsProject.Core.Examples.Entities;
using FluentValidation;

namespace CqrsProject.Core.Examples.UseCases.CreateExample;

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
