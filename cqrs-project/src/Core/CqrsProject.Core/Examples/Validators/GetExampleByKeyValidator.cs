using CqrsProject.Core.Examples.Queries;
using FluentValidation;

namespace CqrsProject.Core.Examples.Validators;

public class GetExampleByKeyValidator : AbstractValidator<GetExampleByKeyQuery>
{
    public GetExampleByKeyValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull();
    }
}
