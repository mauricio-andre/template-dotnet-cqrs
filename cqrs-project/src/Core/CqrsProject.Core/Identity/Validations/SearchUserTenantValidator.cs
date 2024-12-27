using CqrsProject.Core.Identity.Queries;
using FluentValidation;

namespace CqrsProject.Core.Tenants.Validations;

public class SearchUserTenantValidator : AbstractValidator<SearchUserTenantQuery>
{
    public SearchUserTenantValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
