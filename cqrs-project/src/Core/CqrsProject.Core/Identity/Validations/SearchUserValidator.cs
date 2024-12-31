using CqrsProject.Core.Identity.Queries;
using FluentValidation;

namespace CqrsProject.Core.Identity.Validations;

public class SearchUserValidator : AbstractValidator<SearchUserQuery>
{
    public SearchUserValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
