using CqrsProject.Core.Tenants.Queries;
using FluentValidation;

namespace CqrsProject.Core.Tenants.Validations;

public class SearchTenantConnectionStringValidator : AbstractValidator<SearchTenantConnectionStringQuery>
{
    public SearchTenantConnectionStringValidator()
    {
        RuleFor(prop => prop.TenantId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(prop => prop.Take)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
