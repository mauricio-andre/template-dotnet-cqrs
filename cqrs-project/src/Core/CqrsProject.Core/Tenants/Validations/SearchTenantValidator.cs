using CqrsProject.Core.Queries;
using FluentValidation;

namespace CqrsProject.Core.Validations;

public class SearchTenantValidator : AbstractValidator<SearchTenantQuery>
{
    public SearchTenantValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
