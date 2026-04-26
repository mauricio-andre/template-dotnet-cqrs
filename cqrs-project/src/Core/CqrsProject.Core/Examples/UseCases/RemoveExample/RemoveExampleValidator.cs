using FluentValidation;

namespace CqrsProject.Core.Examples.UseCases.RemoveExample;

public class RemoveExampleValidator : AbstractValidator<RemoveExampleCommand>
{
    public RemoveExampleValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull();
    }
}
