using CqrsProject.Core.Identity.Queries;
using FluentValidation;

namespace CqrsProject.Core.Identity.Validations;

public class SearchRoleClaimValidator : AbstractValidator<SearchRoleClaimQuery>
{
    public SearchRoleClaimValidator()
    {
        RuleFor(prop => prop.RoleId)
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
