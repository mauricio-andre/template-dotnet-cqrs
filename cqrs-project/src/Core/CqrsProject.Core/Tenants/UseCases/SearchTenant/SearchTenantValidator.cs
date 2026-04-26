using FluentValidation;

namespace CqrsProject.Core.Tenants.UseCases.SearchTenant;

public class SearchTenantValidator : AbstractValidator<SearchTenantQuery>
{
    public SearchTenantValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
