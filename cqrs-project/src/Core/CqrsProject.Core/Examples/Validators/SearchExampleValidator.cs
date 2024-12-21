using CqrsProject.Core.Examples.Queries;
using FluentValidation;

namespace CqrsProject.Core.Examples.Validators;

public class SearchExampleValidator : AbstractValidator<SearchExampleQuery>
{
    public SearchExampleValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
