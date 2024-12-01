using CqrsProject.Core.Queries;
using FluentValidation;

namespace CqrsProject.Core.Validators;

public class GetTenantByKeyValidator : AbstractValidator<GetTenantByKeyQuery>
{
    public GetTenantByKeyValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull();
    }
}
