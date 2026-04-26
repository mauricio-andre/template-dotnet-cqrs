using FluentValidation;

namespace CqrsProject.Core.UserTenants.UseCases.SearchUserTenant;

public class SearchUserTenantValidator : AbstractValidator<SearchUserTenantQuery>
{
    public SearchUserTenantValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
