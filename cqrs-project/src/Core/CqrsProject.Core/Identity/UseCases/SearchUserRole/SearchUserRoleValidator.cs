using FluentValidation;

namespace CqrsProject.Core.Identity.UseCases.SearchUserRole;

public class SearchUserRoleValidator : AbstractValidator<SearchUserRoleQuery>
{
    public SearchUserRoleValidator()
    {
        RuleFor(prop => prop.Take)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000);

        RuleFor(prop => prop.Skip)
            .GreaterThanOrEqualTo(0);
    }
}
