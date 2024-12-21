using CqrsProject.Core.Tenants.Queries;
using FluentValidation;

namespace CqrsProject.Core.Tenants.Validators;

public class GetTenantByKeyValidator : AbstractValidator<GetTenantByKeyQuery>
{
    public GetTenantByKeyValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
