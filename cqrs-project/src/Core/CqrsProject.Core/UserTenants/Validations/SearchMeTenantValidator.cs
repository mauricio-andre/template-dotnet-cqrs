using CqrsProject.Core.UserTenants.Queries;
using FluentValidation;

namespace CqrsProject.Core.UserTenants.Validations;

public class SearchMeTenantValidator : AbstractValidator<SearchMeTenantQuery>
{
    public SearchMeTenantValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
