using FluentValidation;

namespace CqrsProject.Core.Tenants.UseCases.GetTenantByKey;

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
