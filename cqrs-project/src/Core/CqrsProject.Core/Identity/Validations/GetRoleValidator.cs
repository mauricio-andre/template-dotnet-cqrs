using CqrsProject.Core.Identity.Queries;
using FluentValidation;

namespace CqrsProject.Core.Tenants.Validators;

public class GetRoleValidator : AbstractValidator<GetRoleQuery>
{
    public GetRoleValidator()
    {
        RuleFor(prop => prop.Id)
            .NotEmpty()
            .NotNull()
            .NotEqual(Guid.Empty);
    }
}
