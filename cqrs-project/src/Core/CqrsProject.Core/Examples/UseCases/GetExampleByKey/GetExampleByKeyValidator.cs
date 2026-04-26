using FluentValidation;

namespace CqrsProject.Core.Examples.UseCases.GetExampleByKey;

public class GetExampleByKeyValidator : AbstractValidator<GetExampleByKeyQuery>
{
    public GetExampleByKeyValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull();
    }
}
