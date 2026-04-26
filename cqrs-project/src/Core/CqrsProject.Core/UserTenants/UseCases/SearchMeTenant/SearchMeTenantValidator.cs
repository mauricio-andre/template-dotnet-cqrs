using FluentValidation;

namespace CqrsProject.Core.UserTenants.UseCases.SearchMeTenant;

public class SearchMeTenantValidator : AbstractValidator<SearchMeTenantQuery>
{
    public SearchMeTenantValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
