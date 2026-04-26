using FluentValidation;

namespace CqrsProject.Core.Examples.UseCases.SearchExample;

public class SearchExampleValidator : AbstractValidator<SearchExampleQuery>
{
    public SearchExampleValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
