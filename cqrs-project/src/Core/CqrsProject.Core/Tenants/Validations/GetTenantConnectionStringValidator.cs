using CqrsProject.Core.Tenants.Queries;
using FluentValidation;

namespace CqrsProject.Core.Tenants.Validations;

public class GetTenantConnectionStringValidator : AbstractValidator<GetTenantConnectionStringQuery>
{
    public GetTenantConnectionStringValidator()
    {
        RuleFor(prop => prop.TenantId)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);

        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
