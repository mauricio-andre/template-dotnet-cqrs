using CqrsProject.Core.Queries;
using FluentValidation;

namespace CqrsProject.Core.Validations;

public class SearchTenantConnectionStringValidator : AbstractValidator<SearchTenantConnectionStringQuery>
{
    public SearchTenantConnectionStringValidator()
    {
        RuleFor(prop => prop.TenantId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(prop => prop.Take)
            .GreaterThan(0);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
