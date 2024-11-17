using CqrsProject.Core.Queries;
using FluentValidation;

namespace CqrsProject.Core.Validators;

public class GetExampleByKeyValidator : AbstractValidator<GetExampleByKeyQuery>
{
    public GetExampleByKeyValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull();
    }
}
